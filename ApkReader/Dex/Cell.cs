using System;
using System.Diagnostics;

namespace AlphaOmega.Debug.Dex
{
	/// <summary>Generic cell for dynamic structures</summary>
	[DebuggerDisplay("Column={Column.Name} Value={Value}")]
	public class Cell : ICell
	{
		/// <summary>Here can be cell value or value length or reference to the different table</summary>
		public UInt32 RawValue { get; }

		/// <summary>Abstract value stored in the column</summary>
		public Object Value { get; }

		/// <summary>Description of the column owner</summary>
		public Column Column { get; }
		IColumn ICell.Column => this.Column;

		internal Cell(Column column, UInt32 rawValue, Object value)
		{
			this.Column = column ?? throw new ArgumentNullException(nameof(column));
			this.RawValue = rawValue;
			this.Value = value;

			if(column?.ColumnType != ColumnType.Payload)
				throw new InvalidOperationException("Only ColumnType==Payload is supported through generic constructor");
		}

		internal Cell(DexFile file, Column column, ref UInt32 offset)
		{
			_ = file ?? throw new ArgumentNullException(nameof(file));
			_ = column ?? throw new ArgumentNullException(nameof(column));

			this.Column = column;

			switch(column.ColumnType)
			{
			case ColumnType.Byte:
				Byte bValue = file.Loader.ReadBytes(offset, 1)[0];
				this.RawValue = bValue;
				this.Value = bValue;
				offset++;
				break;
			case ColumnType.UInt16:
				UInt16 sValue = file.PtrToStructure<UInt16>(offset);
				this.RawValue = sValue;
				this.Value = sValue;
				offset += sizeof(UInt16);
				break;
			case ColumnType.UInt32:
				UInt32 iValue = file.PtrToStructure<UInt32>(offset);
				this.RawValue = iValue;
				this.Value = iValue;
				offset += sizeof(UInt32);
				break;
			case ColumnType.SLeb128:
				Int32 slValue = file.ReadSLeb128(ref offset);
				this.RawValue = (UInt32)slValue;//HACK: Here can be signed integer
				this.Value = slValue;
				break;
			case ColumnType.ULeb128:
				Int32 ulValue = file.ReadULeb128(ref offset);
				this.RawValue = checked((UInt32)ulValue);
				this.Value = ulValue;
				break;
			case ColumnType.String:
				Int32 utf16_size = file.ReadULeb128(ref offset);
				this.RawValue = (UInt32)utf16_size;
				this.Value = ReadMUtf8(file, utf16_size, ref offset);
				break;
			case ColumnType.UInt16Array:
				UInt32 itemsCount = this.RawValue = file.Loader.PtrToStructure<UInt32>(offset);
				offset += (UInt32)sizeof(UInt32);
				UInt16[] type_idxs = new UInt16[itemsCount];

				if(itemsCount > 0)
				{
					for(UInt32 loop = 0; loop < itemsCount; loop++)
					{
						type_idxs[loop] = file.Loader.PtrToStructure<UInt16>(offset);
						offset += (UInt32)sizeof(UInt16);
					}

					//TODO: Dex.TYPE.CODE_ITEM padding может быть, а может и не быть... Надо проверять...
					UInt32 p = (UInt32)sizeof(UInt32);
					UInt32 padding = (offset % p) != 0 ? (p - (offset % p)) : 0;
					offset += padding;
				}
				this.Value = type_idxs;
				break;
			case ColumnType.UInt32Array:
				UInt32 itemsCount1 = this.RawValue = file.Loader.PtrToStructure<UInt32>(offset);
				offset += (UInt32)sizeof(UInt32);
				UInt32[] type_idxs1 = new UInt32[itemsCount1];

				for(UInt32 loop = 0; loop < itemsCount1; loop++)
				{
					type_idxs1[loop] = file.Loader.PtrToStructure<UInt32>(offset);
					offset += (UInt32)sizeof(UInt32);
				}
				this.Value = type_idxs1;
				break;
			default:
				throw new NotImplementedException($"Type {column.ColumnType} not implemented");
			}
		}

		private static String ReadMUtf8(DexFile file, Int32 utf16_size, ref UInt32 offset)
		{
			// Calculate worst-case byte size (3 bytes per char + 1 null terminator) to safely read
			var maximumPossibleByteCount = checked((UInt32)utf16_size * 3 + 1);
			var buffer = file.Loader.ReadBytes(offset, maximumPossibleByteCount);

			// Find the actual null terminator to determine the real byte length
			var nullTerminatorIndex = Array.IndexOf(buffer, (Byte)0x00);

			if(nullTerminatorIndex < 0)
				throw new InvalidOperationException("Invalid string_data_item: null terminator not found within the expected bounds.");

			// Advance the global file offset past the string and its null terminator
			offset += (UInt32)nullTerminatorIndex + 1;

			var characterBuffer = new Char[utf16_size];

			// Slice the raw data to exclude the null terminator for decoding
			var encodedData = new Byte[nullTerminatorIndex];
			Array.Copy(buffer, 0, encodedData, 0, nullTerminatorIndex);

			var currentByteIndex = 0;
			var currentCharacterIndex = 0;

			while(currentByteIndex < encodedData.Length)
			{
				var firstByte = encodedData[currentByteIndex];
				currentByteIndex++;

				// Safety check to ensure we don't overflow our output buffer
				if(currentCharacterIndex >= characterBuffer.Length)
					throw new InvalidOperationException("Invalid string_data_item: decoded char count exceeds declared utf16_size.");

				// Case 1: 1-byte sequence (ASCII) - 0xxxxxxx
				if((firstByte & 0x80) == 0)
				{
					characterBuffer[currentCharacterIndex] = (Char)firstByte;
					currentCharacterIndex++;
					continue;
				}

				// Case 2: 2-byte sequence - 110xxxxx 10xxxxxx
				if((firstByte & 0xE0) == 0xC0)
				{
					if(currentByteIndex >= encodedData.Length)
						throw new InvalidOperationException("Invalid MUTF-8: truncated 2-byte sequence.");

					var secondByte = encodedData[currentByteIndex];
					currentByteIndex++;

					// Special Case: MUTF-8 encodes null characters as 2 bytes (0xC0 0x80)
					if(firstByte == 0xC0 && secondByte == 0x80)
					{
						characterBuffer[currentCharacterIndex] = '\0';
					} else
					{
						// Standard 2-byte decoding
						var utf16CodeUnit = ((firstByte & 0x1F) << 6) | (secondByte & 0x3F);
						characterBuffer[currentCharacterIndex] = (Char)utf16CodeUnit;
					}

					currentCharacterIndex++;
					continue;
				}

				// Case 3: 3-byte sequence - 1110xxxx 10xxxxxx 10xxxxxx
				if((firstByte & 0xF0) == 0xE0)
				{
					if(currentByteIndex + 1 >= encodedData.Length)
						throw new InvalidOperationException("Invalid MUTF-8: truncated 3-byte sequence.");

					var secondByte = encodedData[currentByteIndex];
					var thirdByte = encodedData[currentByteIndex + 1];
					currentByteIndex += 2;

					// Standard 3-byte decoding
					var utf16CodeUnit = ((firstByte & 0x0F) << 12) | ((secondByte & 0x3F) << 6) | (thirdByte & 0x3F);
					characterBuffer[currentCharacterIndex] = (Char)utf16CodeUnit;
					currentCharacterIndex++;
					continue;
				}

				// DEX uses MUTF-8 which does not support standard UTF-8 4-byte sequences
				throw new InvalidOperationException("Invalid MUTF-8: 4-byte sequences are not used in DEX strings.");
			}

			// Ensure the number of decoded characters matches the declared ULEB128 size
			if(currentCharacterIndex != utf16_size)
				throw new InvalidOperationException("Invalid string_data_item: decoded UTF-16 length does not match utf16_size.");

			return new String(characterBuffer);
		}
	}
}