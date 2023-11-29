using System;
using System.Collections;
using System.Collections.Generic;

namespace AlphaOmega.Debug.Dex
{
	/// <summary>Basic table for the strongly typed generic table</summary>
	/// <typeparam name="T">Strongly typed row description</typeparam>
	public class BaseTable<T> : IEnumerable<T> where T : BaseRow, new()
	{
		private readonly DexFile _file;

		/// <summary>Generic basic table</summary>
		public Table Table { get; }

		/// <summary>Gets row by row index</summary>
		/// <param name="rowIndex">Row index from current table</param>
		/// <returns>Strongly typed row by index</returns>
		public T this[UInt32 rowIndex] => new T() { Row = this.Table[rowIndex], File = this._file, };

		internal BaseTable(DexFile file, TableType type)
		{
			this._file = file ?? throw new ArgumentNullException(nameof(file));
			this.Table = this._file.GetSectionTable(type);
		}

		/// <summary>Get row by file ofsset</summary>
		/// <param name="offset">Offset from beginning of the file</param>
		/// <returns>Strongly typed row</returns>
		public T GetRowByOffset(UInt32 offset)
			=> new T() { Row = this.Table.GetRowByOffset(offset), File = this._file, };

		/// <summary>Get all strongly typed data from generaic table</summary>
		/// <returns>Detailed strongly typed rows from current table</returns>
		public IEnumerator<T> GetEnumerator()
		{
			foreach(var row in this.Table.Rows)
				yield return new T() { Row = row, File = this._file, };
		}

		IEnumerator IEnumerable.GetEnumerator()
			=> this.GetEnumerator();
	}
}