using System;
using System.Diagnostics;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>Referenced from class_def_item and proto_id_item</summary>
	[DebuggerDisplay("type_idx={" + nameof(type_idxI) + "}")]
	public class type_list_row : BaseRow
	{
		private UInt16[] type_idxI => base.GetValue<UInt16[]>(0);

		/// <summary>Index into the type_ids list</summary>
		public type_id_row[] type_idx
		{
			get
			{
				BaseTable<Dex.Tables.type_id_row> table = base.File.TYPE_ID_ITEM;
				return Array.ConvertAll(this.type_idxI, delegate (UInt16 index) { return table[index]; });
			}
		}
	}
}