using System;
using System.Diagnostics;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>
	/// String identifiers list.
	/// These are identifiers for all the strings used by this file, either for internal naming (e.g., type descriptors) or as constant objects referred to by code.
	/// </summary>
	/// <remarks>This list must be sorted by string contents, using UTF-16 code point values (not in a locale-sensitive manner), and it must not contain any duplicate entries.</remarks>
	[DebuggerDisplay("data={" + nameof(data) + "}")]
	public class string_data_row : BaseRow
	{
		/// <summary>
		/// A series of MUTF-8 code units (a.k.a. octets, a.k.a. bytes) followed by a byte of value 0.
		/// See "MUTF-8 (Modified UTF-8) Encoding" above for details and discussion about the data format.
		/// </summary>
		/// <remarks>
		/// It is acceptable to have a string which includes (the encoded form of) UTF-16 surrogate code units (that is, U+d800 … U+dfff) either in isolation or out-of-order with respect to the usual encoding of Unicode into UTF-16.
		/// It is up to higher-level uses of strings to reject such invalid encodings, if appropriate.
		/// </remarks>
		public String data => base.GetValue<String>(0);
	}
}