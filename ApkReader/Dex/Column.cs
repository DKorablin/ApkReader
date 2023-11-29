using System;
using System.Diagnostics;

namespace AlphaOmega.Debug.Dex
{
	/// <summary>Generic colum for dynamic structures</summary>
	[DebuggerDisplay("{Name} ({" + nameof(ColumnType) + "})")]
	public class Column : IColumn
	{
		/// <summary>Zero based index from the beggining of structure</summary>
		public UInt16 Index { get; }

		/// <summary>Name of the column</summary>
		public String Name { get; }

		/// <summary>Type of data in the current column</summary>
		public ColumnType ColumnType { get; }

		/// <summary>Create instance of the column</summary>
		/// <param name="index">Index of a column in the generic table</param>
		/// <param name="name">Name of the column</param>
		/// <param name="columnType">ype of data in the current column</param>
		internal Column(UInt16 index, String name, ColumnType columnType)
		{
			this.Index = index;
			this.Name = name;
			this.ColumnType = columnType;
		}
	}
}