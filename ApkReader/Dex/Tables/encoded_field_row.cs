using System;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>Static and instance fields from the class_data_item</summary>
	public class encoded_field_row : BaseRow
	{
		/// <summary>
		/// Index into the field_ids list for the identity of this field (includes the name and descriptor), represented as a difference from the index of previous element in the list.
		/// The index of the first element in a list is represented directly.
		/// </summary>
		public UInt32 field_idx_diff { get { return (UInt32)base.GetValue<Int32>(0); } }

		/// <summary>Access flags for the field (public, final, etc.).</summary>
		public DexApi.ACC access_flags { get { return (DexApi.ACC)base.GetValue<Int32>(1); } }
	}
}