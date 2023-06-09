﻿using System;

namespace AlphaOmega.Debug.Dex
{
	/// <summary>Basic row for the strongly typed structures</summary>
	public class BaseRow : IBaseRow
	{
		private Row _row;
		private DexFile _file;

		internal Row Row
		{
			get { return this._row; }
			set { this._row = value ?? throw new ArgumentNullException(nameof(value)); }
		}
		IRow IBaseRow.Row { get { return this._row; } }

		/// <summary>Row index</summary>
		internal UInt32 RowIndex { get { return this.Row.Index; } }

		/// <summary>Offset from the beginning of the image</summary>
		internal UInt32 RowOffset { get { return this.Row.Offset; } }

		internal DexFile File
		{
			get { return this._file; }
			set { this._file = value ?? throw new ArgumentNullException(nameof(value)); }
		}

		/// <summary>Get value from base generic table by columnIndex</summary>
		/// <typeparam name="T">Type of value by column index stored in the generic table</typeparam>
		/// <param name="columnIndex">Column index from generic table</param>
		/// <returns>Stored typed value stored in the cell by column index</returns>
		protected T GetValue<T>(UInt16 columnIndex)
		{
			return (T)this.Row[columnIndex].Value;
		}
	}
}