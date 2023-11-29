using System;

namespace AlphaOmega.Debug.Dex
{
	/// <summary>Reference to the another table</summary>
	public class RowReference
	{
		/// <summary>Owner DEX file description</summary>
		private DexFile File { get; }

		/// <summary>Reference table type</summary>
		public TableType Type { get; }

		/// <summary>Reference row index</summary>
		public UInt32 RowIndex { get; }

		internal RowReference(DexFile file, TableType type, UInt32 rowIndex)
		{
			this.File = file;
			this.Type = type;
			this.RowIndex = rowIndex;
		}

		/// <summary>Get row reference</summary>
		/// <returns>Row where reference points</returns>
		public Row GetReference()
			=> this.File.GetSectionTable(this.Type)[this.RowIndex];
	}
}