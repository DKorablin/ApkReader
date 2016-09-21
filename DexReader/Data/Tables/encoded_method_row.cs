using System;

namespace AlphaOmega.Debug.Data.Tables
{
	/// <summary>Class method definition list</summary>
	public class encoded_method_row : BaseRow
	{
		/// <summary>
		/// Index into the method_ids list for the identity of this method (includes the name and descriptor), represented as a difference from the index of previous element in the list.
		/// </summary>
		/// <remarks>The index of the first element in a list is represented directly.</remarks>
		public UInt32 method_idx_diff { get { return (UInt32)base.GetValue<Int32>(0); } }

		/// <summary>Access flags for the method (public, final, etc.).</summary>
		public Dex.ACC access_flags { get { return (Dex.ACC)base.GetValue<Int32>(1); } }

		private UInt32 code_offI { get { return (UInt32)base.GetValue<Int32>(2); } }

		/// <summary>
		/// Offset from the start of the file to the code structure for this method, or 0 if this method is either abstract or native.
		/// The offset should be to a location in the data section.
		/// </summary>
		public code_row code_off
		{
			get
			{
				return this.code_offI == 0
					? null
					: base.File.CODE_ITEM.GetRowByOffset(this.code_offI);
			}
		}
	}
}