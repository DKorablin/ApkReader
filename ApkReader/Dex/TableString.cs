using System;

namespace AlphaOmega.Debug.Dex
{
	internal class TableString : Table
	{
		public TableString()
			: base(TableType.STRING_DATA_ITEM)
		{ }

		protected internal override Cell ReadCellPayload(DexFile file, UInt32 rowIndex, Column column, Cell[] cells, ref UInt32 offset)
		{
			offset = file.STRING_ID_ITEM[rowIndex].string_data_off;
			return base.ReadCellPayload(file, rowIndex, column, cells, ref offset);
		}

		/*protected internal override Boolean AddRowToTable(UInt32 rowIndex, UInt32 offset, Cell[] cells)
		{
			return cells[0].RawValue == 0
				? false
				: base.AddRowToTable(rowIndex, offset, cells);
		}*/
	}
}