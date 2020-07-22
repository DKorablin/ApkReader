using System;
using AlphaOmega.Debug.Dex.Tables;

namespace AlphaOmega.Debug.Dex
{
	internal class TableCode : Table
	{
		public TableCode()
			: base(TableType.CODE_ITEM)
		{ }

		protected internal override Cell ReadCellPayload(DexFile file, UInt32 rowIndex, Column column, Cell[] cells, ref uint offset)
		{
			Cell result;
			switch((code_row.columns)column.Index)
			{
			case code_row.columns.tries:
				UInt32 tries_size = cells[(UInt32)code_row.columns.tries_size].RawValue;
				UInt32[] indexes = file.GetSectionTable(TableType.try_item).ReadInnerSection(file, ref offset, tries_size);
				result = new Cell(column, tries_size, indexes);
				break;
			case code_row.columns.handlers:
				UInt32 tries_size1 = cells[(UInt32)code_row.columns.tries_size].RawValue;
				if(tries_size1 > 0)
				{
					Int32 handler_list_size = file.ReadULeb128(ref offset);
					UInt32[] indexes1 = file.GetSectionTable(TableType.encoded_catch_handler_list).ReadInnerSection(file, ref offset, (UInt32)handler_list_size);

					UInt32 p = (UInt32)sizeof(UInt32);
					UInt32 padding = ((offset) % p) != 0 ? (p - (offset) % p) : 0;
					offset += padding;

					result = new Cell(column, tries_size1, indexes1);
					//throw new NotImplementedException();
				} else
					result = new Cell(column, 0, null);
				break;
			default:
				result = base.ReadCellPayload(file, rowIndex, column, cells, ref offset);
				break;
			}
			return result;
		}
	}
}