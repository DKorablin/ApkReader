using System;
using System.Diagnostics;

namespace AlphaOmega.Debug.Data.Tables
{
	/// <summary>banana banana banana</summary>
	[DebuggerDisplay("Count: {annotation_off}")]
	public class annotation_set_row : BaseRow
	{
		/// <summary>Offset from the start of the file to an annotation.</summary>
		/// <remarks>The offset should be to a location in the data section, and the format of the data at that location is specified by "annotation_item" below.</remarks>
		public UInt32[] annotation_off { get { return base.GetValue<UInt32[]>(0); } }
	}
}