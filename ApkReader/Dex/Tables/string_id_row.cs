using System;
using System.Diagnostics;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>
	/// String identifiers list.
	/// These are identifiers for all the strings used by this file, either for internal naming (e.g., type descriptors) or as constant objects referred to by code.
	/// </summary>
	/// <remarks>This list must be sorted by string contents, using UTF-16 code point values (not in a locale-sensitive manner), and it must not contain any duplicate entries.</remarks>
	[DebuggerDisplay("string_data_off={" + nameof(string_data_off) + "}")]
	public class string_id_row : BaseRow
	{
		/// <summary>
		/// offset from the start of the file to the string data for this item.
		/// The offset should be to a location in the data section, and the data should be in the format specified by "string_data_item" below.
		/// </summary>
		/// <remarks>There is no alignment requirement for the offset.</remarks>
		public UInt32 string_data_off => base.GetValue<UInt32>(0);
	}
}