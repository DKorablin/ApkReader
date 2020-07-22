using System;
using System.Diagnostics;
using System.Text;

namespace AlphaOmega.Debug.Dex
{
	/// <summary>Generic cell for dynamic structures</summary>
	[DebuggerDisplay("Column={Column.Name} Value={Value}")]
	public class Cell : ICell
	{
		#region Fields
		private readonly Object _value;
		private readonly UInt32 _rawValue;
		private readonly Column _column;
		#endregion Fields

		/// <summary>Here can be cell value or value length or reference to the different table</summary>
		public UInt32 RawValue { get { return this._rawValue; } }

		/// <summary>Abstract value stored in the column</summary>
		public Object Value { get { return this._value; } }

		/// <summary>Description of the column owner</summary>
		public Column Column { get { return this._column; } }
		IColumn ICell.Column { get { return this._column; } }

		internal Cell(Column column, UInt32 rawValue, Object value)
		{
			if(column == null)
				throw new NotImplementedException("column");
			if(column.ColumnType != ColumnType.Payload)
				throw new ArgumentException("Only ColumnType==Payload is supported through generic constructor");

			this._column = column;
			this._rawValue = rawValue;
			this._value = value;
		}

		internal Cell(DexFile file, Column column, ref UInt32 offset)
		{
			if(file == null)
				throw new ArgumentNullException("file");
			if(column == null)
				throw new ArgumentNullException("column");

			this._column = column;

			switch(column.ColumnType)
			{
			case ColumnType.Byte:
				Byte bValue = file.Loader.ReadBytes(offset, 1)[0];
				this._rawValue = bValue;
				this._value = bValue;
				offset++;
				break;
			case ColumnType.UInt16:
				UInt16 sValue = file.PtrToStructure<UInt16>(offset);
				this._rawValue = sValue;
				this._value = sValue;
				offset += sizeof(UInt16);
				break;
			case ColumnType.UInt32:
				UInt32 iValue = file.PtrToStructure<UInt32>(offset);
				this._rawValue = iValue;
				this._value = iValue;
				offset += sizeof(UInt32);
				break;
			case ColumnType.SLeb128:
				Int32 slValue = file.ReadSLeb128(ref offset);
				this._rawValue = (UInt32)slValue;//HACK: Here can be signed integer
				this._value = slValue;
				break;
			case ColumnType.ULeb128:
				Int32 ulValue = file.ReadULeb128(ref offset);
				this._rawValue = checked((UInt32)ulValue);
				this._value = ulValue;
				break;
			case ColumnType.String:
				Int32 utf16_size = file.ReadULeb128(ref offset);
				this._rawValue = (UInt32)utf16_size;
				if(utf16_size > 0)
				{
					Byte[] bytes = file.Loader.ReadBytes(offset, (UInt32)utf16_size);
					this._value = Encoding.UTF8.GetString(bytes);

					offset += (UInt32)utf16_size;
				}
				break;
			case ColumnType.UInt16Array:
				UInt32 itemsCount = this._rawValue = file.Loader.PtrToStructure<UInt32>(offset);
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
					UInt32 padding = ((offset) % p) != 0 ? (p - (offset) % p) : 0;
					offset += padding;
				}
				this._value = type_idxs;
				break;
			case ColumnType.UInt32Array:
				UInt32 itemsCount1 = this._rawValue = file.Loader.PtrToStructure<UInt32>(offset);
				offset += (UInt32)sizeof(UInt32);
				UInt32[] type_idxs1 = new UInt32[itemsCount1];

				for(UInt32 loop = 0; loop < itemsCount1; loop++)
				{
					type_idxs1[loop] = file.Loader.PtrToStructure<UInt32>(offset);
					offset += (UInt32)sizeof(UInt32);
				}
				this._value = type_idxs1;
				break;
			default:
				throw new NotImplementedException(String.Format("Type {0} not implemented", column.ColumnType));
			}
		}
	}
}