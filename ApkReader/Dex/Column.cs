using System;
using System.Diagnostics;

namespace AlphaOmega.Debug.Dex
{
	/// <summary>Generic colum for dynamic structures</summary>
	[DebuggerDisplay("{Name} ({ColumnType})")]
	public class Column : IColumn
	{
		#region Fields
		private readonly UInt16 _index;
		private readonly String _name;
		private readonly ColumnType _columnType;
		#endregion Fields

		/// <summary>Zero based index from the beggining of structure</summary>
		public UInt16 Index { get { return this._index; } }

		/// <summary>Name of the column</summary>
		public String Name { get { return this._name; } }

		/// <summary>Type of data in the current column</summary>
		public ColumnType ColumnType { get { return this._columnType; } }

		/// <summary>Create instance of the column</summary>
		/// <param name="index">Index of a column in the generic table</param>
		/// <param name="name">Name of the column</param>
		/// <param name="columnType">ype of data in the current column</param>
		internal Column(UInt16 index, String name, ColumnType columnType)
		{
			this._index = index;
			this._name = name;
			this._columnType = columnType;
		}
	}
}