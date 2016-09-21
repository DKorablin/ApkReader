using System;
using System.Diagnostics;

namespace AlphaOmega.Debug.Data.Tables
{
	/// <summary>Referenced from class_def_item and proto_id_item</summary>
	[DebuggerDisplay("type_idx={type_idxI}")]
	public class type_list_row : BaseRow
	{
		private UInt16[] type_idxI { get { return base.GetValue<UInt16[]>(0); } }

		/// <summary>Index into the type_ids list</summary>
		public type_id_row[] type_idx
		{
			get
			{
				var table = base.File.TYPE_ID_ITEM;
				return Array.ConvertAll(this.type_idxI, delegate(UInt16 index) { return table[index]; });
			}
		}
	}
}