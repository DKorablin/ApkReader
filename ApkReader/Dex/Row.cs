using System;
using System.Collections.Generic;
using System.Collections;

namespace AlphaOmega.Debug.Dex
{
	/// <summary>Generic row for variable structure collection</summary>
	public class Row : IRow
	{
		/// <summary>Row index</summary>
		public UInt32 Index { get; }

		/// <summary>Offset from the beggining of the current file</summary>
		public UInt32 Offset { get; }

		/// <summary>Row cells</summary>
		public Cell[] Cells { get; }
		ICell[] IRow.Cells => this.Cells;

		/// <summary>Row owner table</summary>
		public Table Table { get; }
		ITable IRow.Table => this.Table;

		/// <summary>Get cell by column index</summary>
		/// <param name="columnIndex">Index of column from current table</param>
		/// <returns>Cell from current row by column index</returns>
		public Cell this[UInt16 columnIndex]
			=> columnIndex < this.Cells.Length
				? this.Cells[columnIndex]
				: throw new ArgumentOutOfRangeException("columnIndex is to big");

		ICell IRow.this[UInt16 columnIndex] => this[columnIndex];

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

				foreach(Cell cell in this.Cells)
					if(cell.Column.Name == columnName)
						return cell;
				throw new ArgumentException($"Column with name '{columnName}' not found");
			}
		}
		ICell IRow.this[String columnName] => this[columnName];
		ICell IRow.this[IColumn column] => this[column.Index];

		internal Row(Table table, UInt32 rowIndex, UInt32 offset, Cell[] cells)
		{
			this.Table = table;
			this.Index = rowIndex;
			this.Offset = offset;
			this.Cells = cells;
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
			=> this.GetEnumerator();
	}
}