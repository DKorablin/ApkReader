using System;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>
	/// Represents a method annotation item in a DEX file.
	/// 
	/// A method annotation item associates annotations with a specific method defined in a class.
	/// It specifies which method is being annotated and provides an offset to the corresponding
	/// annotation set that contains the actual annotations applied to that method.
	/// 
	/// This structure is used in the method_annotations section of an annotation directory
	/// to document annotations applied to individual methods (such as @Deprecated, @Override, etc.).
	/// </summary>
	public class method_annotation_row : BaseRow
	{
		private UInt32 method_idxI => base.GetValue<UInt32>(0);

		/// <summary>Gets the method row referenced by this method annotation.</summary>
		/// <remarks>
		/// Returns the method_id_row that identifies which method is being annotated.
		/// This provides access to the full method definition and metadata.
		/// </remarks>
		public method_id_row method_idx => base.File.METHOD_ID_ITEM[this.method_idxI];

		private UInt32 annotations_offI => base.GetValue<UInt32>(1);

		/// <summary>Gets the annotation set for this method.</summary>
		/// <remarks>
		/// Returns the annotation_set_row located at the specified offset from the file start.
		/// The annotation set contains all annotations applied to this method.
		/// 
		/// The offset points to a location in the data section and follows the "annotation_set_item" format,
		/// which is an array of offsets to individual annotation items.
		/// </remarks>
		public annotation_set_row annotations_off => base.File.ANNOTATION_SET_ITEM.GetRowByOffset(this.annotations_offI);
	}
}