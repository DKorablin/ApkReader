using System;
using System.Collections.Generic;

namespace AlphaOmega.Debug.Dex
{
	/// <summary>Generic structure container</summary>
	public class Table : ITable
	{
		#region Fields
		private readonly TableType _type;
		private readonly Column[] _columns;
		private readonly Dictionary<UInt32, Row> _rows;
		#endregion Fields

		/// <summary>All rows from current table</summary>
		public IEnumerable<Row> Rows { get { return this._rows.Values; } }
		IEnumerable<IRow> ITable.Rows
		{
			get
			{
				foreach(Row row in this._rows.Values)
					yield return row;
			}
		}

		/// <summary>Gets specified row by index and check that row from current table</summary>
		/// <param name="rowIndex">Row index</param>
		/// <returns>Row by index from current table</returns>
		/// <exception cref="ArgumentException">Row not found in this table</exception>
		public Row this[UInt32 rowIndex]
		{
			get
			{
				return this._rows[rowIndex];
				/*foreach(Row row in this._rows)
					if(row.Index == rowIndex)
						return row;

				throw new ArgumentException(String.Format("Row with index {0} not found", rowIndex));*/
			}
		}

		IRow ITable.this[UInt32 rowIndex] { get { return this[rowIndex]; } }

		/// <summary>Rows count in the table</summary>
		public UInt32 RowsCount { get { return (UInt32)this._rows.Count; } }

		/// <summary>Columns from current table</summary>
		public Column[] Columns { get { return this._columns; } }
		IColumn[] ITable.Columns { get { return this._columns; } }

		/// <summary>Type of current table</summary>
		public TableType Type { get { return this._type; } }
		Object ITable.Type { get { return this._type; } }

		internal Table(TableType type)
		{
			this._type = type;
			this._columns = Constant.GetTableColumns(type);
			this._rows = new Dictionary<UInt32, Row>();
		}

		internal Row GetRowByOffset(UInt32 offset)
		{
			foreach(Row row in this.Rows)
				if(row.Offset == offset)
					return row;

			return null;
		}

		internal virtual void ReadSection(DexFile file, UInt32 offset, UInt32 maxItems)
		{
			UInt32 rowIndex = 0;

			while(rowIndex < maxItems)
			{
				UInt32 startOffset = offset;
				Cell[] cells = new Cell[this.Columns.Length];
				for(UInt32 loop = 0; loop < cells.Length; loop++)
				{
					Column column = this.Columns[loop];
					cells[loop] = this.ReadCellPayload(file, rowIndex, column, cells, ref offset);
				}

				if(this.AddRowToTable(rowIndex, startOffset, cells))
					rowIndex++;
			}
		}

		/// <summary>Read data from section and returns indexes of added rows</summary>
		/// <param name="file">DEX file reader</param>
		/// <param name="offset">Offset from begginging of the file</param>
		/// <param name="maxItems">Maximum structures in the section</param>
		/// <returns>Row indexes</returns>
		protected internal UInt32[] ReadInnerSection(DexFile file, ref UInt32 offset, UInt32 maxItems)
		{
			UInt32 rowIndex = this.RowsCount;
			UInt32 index = 0;
			List<UInt32> result = new List<UInt32>();

			while(index < maxItems)
			{
				UInt32 startOffset = offset;
				Cell[] cells = new Cell[this.Columns.Length];
				for(UInt32 loop = 0; loop < cells.Length; loop++)
				{
					Column column = this.Columns[loop];
					cells[loop] = this.ReadCellPayload(file, rowIndex, column, cells, ref offset);
				}

				if(this.AddRowToTable(rowIndex, startOffset, cells))
				{
					result.Add(rowIndex);
					index++;
					rowIndex++;
				}
			}

			return result.ToArray();
		}

		/// <summary>Add row to current table</summary>
		/// <param name="rowIndex">New row index</param>
		/// <param name="offset">Offst from beggining of current file</param>
		/// <param name="cells">Row cells</param>
		/// <returns>Returns true if row added successfully</returns>
		protected internal virtual Boolean AddRowToTable(UInt32 rowIndex, UInt32 offset, Cell[] cells)
		{
			Row row = new Row(this, rowIndex, offset, cells);
			this._rows.Add(rowIndex, row);
			return true;
		}

		/// <summary>Read cell data from loaded file</summary>
		/// <param name="file">DEX file</param>
		/// <param name="rowIndex">Row index</param>
		/// <param name="column">Cell column</param>
		/// <param name="cells">All cells from current row</param>
		/// <param name="offset">Offset from beggining of current file</param>
		/// <returns>Structured data</returns>
		protected internal virtual Cell ReadCellPayload(DexFile file, UInt32 rowIndex, Column column, Cell[] cells, ref UInt32 offset)
		{
			return new Cell(file, column, ref offset);
		}
	}
}