using System;

namespace AlphaOmega.Debug.Data
{
	internal class TableString : Table
	{
		internal TableString()
			: base(TableType.STRING_DATA_ITEM)
		{ }

		protected internal override Boolean AddRowToTable(UInt32 rowIndex, UInt32 offset, Cell[] cells)
		{
			return cells[0].RawValue == 0
				? false
				: base.AddRowToTable(rowIndex, offset, cells);
		}
	}
}