using System;
using System.Collections.Generic;
using System.Collections;

namespace AlphaOmega.Debug.Dex
{
	/// <summary>Generic row for variable structure collection</summary>
	public class Row : IRow
	{
		#region Fields
		private readonly Table _table;
		private readonly UInt32 _rowIndex;
		private readonly UInt32 _offset;
		private readonly Cell[] _cells;
		#endregion Fields

		/// <summary>Row index</summary>
		public UInt32 Index { get { return this._rowIndex; } }

		/// <summary>Offset from the beggining of the current file</summary>
		public UInt32 Offset { get { return this._offset; } }

		/// <summary>Row cells</summary>
		public Cell[] Cells { get { return this._cells; } }
		ICell[] IRow.Cells { get { return this._cells; } }

		/// <summary>Row owner table</summary>
		public Table Table { get { return this._table; } }
		ITable IRow.Table { get { return this._table; } }

		/// <summary>Get cell by column index</summary>
		/// <param name="columnIndex">Index of column from current table</param>
		/// <returns>Cell from current row by column index</returns>
		public Cell this[UInt16 columnIndex]
		{
			get
			{
				if(columnIndex < this._cells.Length)
					return this._cells[columnIndex];
				else
					throw new ArgumentOutOfRangeException("columnIndex is to big");
			}
		}
		ICell IRow.this[UInt16 columnIndex] { get { return this[columnIndex]; } }

		/// <summary>Get the cell by column name</summary>
		/// <param name="columnName">Column name</param>
		/// <exception cref="ArgumentNullException">columnName is null</exception>
		/// <exception cref="ArgumentException">Column with specific name not found</exception>
		/// <returns>Cell in specified column index</returns>
		public Cell this[String columnName]
		{
			get
			{
				if(String.IsNullOrEmpty(columnName))
					throw new ArgumentNullException(nameof(columnName));

				foreach(Cell cell in this._cells)
					if(cell.Column.Name == columnName)
						return cell;
				throw new ArgumentException($"Column with name '{columnName}' not found");
			}
		}
		ICell IRow.this[String columnName] { get { return this[columnName]; } }
		ICell IRow.this[IColumn column] { get { return this[column.Index]; } }

		internal Row(Table table, UInt32 rowIndex, UInt32 offset, Cell[] cells)
		{
			this._table = table;
			this._rowIndex = rowIndex;
			this._offset = offset;
			this._cells = cells;
		}

		/// <summary>Get all cells from current fow</summary>
		/// <returns>All cells from row</returns>
		public IEnumerator<Cell> GetEnumerator()
		{
			foreach(Cell cell in this.Cells)
				yield return cell;
		}
		IEnumerator<ICell> IEnumerable<ICell>.GetEnumerator()
		{
			foreach(ICell cell in this.Cells)
				yield return cell;
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}