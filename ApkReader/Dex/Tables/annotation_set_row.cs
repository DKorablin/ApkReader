using System;
using System.Diagnostics;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>Represents an annotation set item in a DEX file.</summary>
	/// <remarks>
	/// An annotation set is a collection of annotation items applied to a single element
	/// (such as a class, field, method, or method parameter).
	/// It contains an array of offsets that reference individual annotation items, each describing a specific annotation
	/// applied to that element.
	/// </remarks>
	[DebuggerDisplay("Count: {"+nameof(annotation_off)+"}")]
	public class annotation_set_row : BaseRow
	{
		/// <summary>
		/// Gets the array of offsets to individual annotation items in this set.
		/// 
		/// Each offset in the array points to an annotation_item structure in the file's data section
		/// that describes a specific annotation applied to the associated element.
		/// </summary>
		/// <remarks>
		/// The offsets should all point to locations in the data section, with the format specified by "annotation_item" in the DEX specification.
		/// The offsets are typically maintained in sorted order.
		/// </remarks>
		public UInt32[] annotation_off => base.GetValue<UInt32[]>(0);
	}
}