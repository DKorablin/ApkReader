using System;

namespace AlphaOmega.Debug.Dex
{
	internal class TableEncodedCatchHandler : Table
	{
		public TableEncodedCatchHandler()
			: base(TableType.encoded_catch_handler_list)
		{ }

		protected internal override Cell ReadCellPayload(DexFile file,UInt32 rowIndex, Column column, Cell[] cells, ref UInt32 offset)
		{
			Cell result;
			Tables.encoded_catch_handler_row.columns colIdx = (Tables.encoded_catch_handler_row.columns)column.Index;
			switch(colIdx)
			{
			case Tables.encoded_catch_handler_row.columns.handlers:
				Int32 handler_size = (Int32)cells[(UInt32)Tables.encoded_catch_handler_row.columns.size].Value;
				UInt32[] indexes = file.GetSectionTable(TableType.encoded_type_addr_pair).ReadInnerSection(file, ref offset, (UInt32)Math.Abs(handler_size));
				result = new Cell(column, (UInt32)indexes.Length, indexes);
				break;
			case Tables.encoded_catch_handler_row.columns.catch_all_addr:
				Int32 handler_size1 = (Int32)cells[(UInt32)Tables.encoded_catch_handler_row.columns.size].Value;
				if(handler_size1 <= 0)
				{
					Int32 catch_add_addr = file.ReadULeb128(ref offset);
					result = new Cell(column, (UInt32)catch_add_addr, (UInt32)catch_add_addr);
				} else
					result = new Cell(column, 0, null);
				break;
			default:
				result=base.ReadCellPayload(file, rowIndex, column, cells, ref offset);
				break;
			}

			return result;
		}
	}
}