using System;

namespace AlphaOmega.Debug.Data
{
	/// <summary>Reference to the another table</summary>
	public class RowReference
	{
		#region Fields
		private readonly DexFile _file;
		private readonly TableType _type;
		private readonly UInt32 _rowIndex;
		#endregion Fields

		/// <summary>Owner DEX file description</summary>
		private DexFile File { get { return this._file; } }

		/// <summary>Reference table type</summary>
		public TableType Type { get { return this._type; } }

		/// <summary>Reference row index</summary>
		public UInt32 RowIndex { get { return this._rowIndex; } }

		internal RowReference(DexFile file, TableType type, UInt32 rowIndex)
		{
			this._file = file;
			this._type = type;
			this._rowIndex = rowIndex;
		}

		/// <summary>Get row reference</summary>
		/// <returns>Row where reference points</returns>
		public Row GetReference()
		{
			return this.File.GetSectionTable(this.Type)[this.RowIndex];
		}
	}
}