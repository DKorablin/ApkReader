using System;
using System.Diagnostics;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>
	/// Method prototype identifiers list.
	/// These are identifiers for all prototypes referred to by this file.
	/// </summary>
	/// <remarks>
	/// This list must be sorted in return-type (by type_id index) major order, and then by argument list (lexicographic ordering, individual arguments ordered by type_id index).
	/// The list must not contain any duplicate entries.
	/// </remarks>
	[DebuggerDisplay("{shorty_idx.data} returns {return_type_idx.descriptor_idx.data}")]
	public class proto_id_row : BaseRow
	{
		private UInt32 shorty_idxI => base.GetValue<UInt32>(0);

		private UInt32 return_type_idxI => base.GetValue<UInt32>(1);

		/// <summary>
		/// Index into the string_ids list for the short-form descriptor string of this prototype.
		/// The string must conform to the syntax for ShortyDescriptor, defined above, and must correspond to the return type and parameters of this item.
		/// </summary>
		public string_data_row shorty_idx => base.File.STRING_DATA_ITEM[this.shorty_idxI];

		/// <summary>Index into the type_ids list for the return type of this prototype.</summary>
		public type_id_row return_type_idx => base.File.TYPE_ID_ITEM[this.return_type_idxI];

		private UInt32 parameters_offI => base.GetValue<UInt32>(2);

		/// <summary>
		/// Offset from the start of the file to the list of parameter types for this prototype, or 0 if this prototype has no parameters.
		/// This offset, if non-zero, should be in the data section, and the data there should be in the format specified by "type_list" below.
		/// Additionally, there should be no reference to the type void in the list.
		/// </summary>
		public type_list_row parameters_off
			=> this.parameters_offI == 0
				? null
				: base.File.TYPE_LIST.GetRowByOffset(this.parameters_offI);
	}
}