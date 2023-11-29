using System;
using System.Diagnostics;
using System.Text;

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
				if(utf16_size > 0)
				{
					Byte[] bytes = file.Loader.ReadBytes(offset, (UInt32)utf16_size);
					this.Value = Encoding.UTF8.GetString(bytes);

					offset += (UInt32)utf16_size;
				}
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
	}
}