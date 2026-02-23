using System;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>Represents a field annotation item in a DEX file.</summary>
	/// <remarks>
	/// A field annotation item associates annotations with a specific field defined in a class.
	/// It specifies which field is being annotated and provides an offset to the corresponding
	/// annotation set that contains the actual annotations applied to that field.
	/// 
	/// This structure is used in the field_annotations section of an annotation directory
	/// to document annotations applied to individual fields (such as @Deprecated, @NonNull, etc.).
	/// </remarks>
	public class field_annotation_row : BaseRow
	{
		private UInt32 field_idxI => base.GetValue<UInt32>(0);

		/// <summary>Gets the field row referenced by this field annotation.</summary>
		/// <remarks>
		/// Returns the field_id_row that identifies which field is being annotated.
		/// This provides access to the full field definition and metadata.
		/// </remarks>
		public field_id_row field_idx => base.File.FIELD_ID_ITEM[this.field_idxI];

		private UInt32 annotations_offI => base.GetValue<UInt32>(1);

		/// <summary>Gets the annotation set for this field.</summary>
		/// <remarks>
		/// Returns the annotation_set_row located at the specified offset from the file start.
		/// The annotation set contains all annotations applied to this field.
		/// 
		/// The offset points to a location in the data section and follows the "annotation_set_item" format,
		/// which is an array of offsets to individual annotation items.
		/// </remarks>
		public annotation_set_row annotations_off => base.File.ANNOTATION_SET_ITEM.GetRowByOffset(this.annotations_offI);
	}
}