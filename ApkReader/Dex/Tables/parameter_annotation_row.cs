using System;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>Represents a parameter annotation row in a DEX file.</summary>
	/// <remarks>
	/// A parameter annotation item associates annotations with specific method parameters.
	/// It specifies which method's parameters are being annotated and provides an offset to the
	/// corresponding annotation set reference list that contains the actual annotations for each parameter.
	/// 
	/// This structure is used in the parameter_annotations section of a DEX file to document
	/// annotations applied to individual method parameters (beyond the method-level annotations).
	/// </remarks>
	public class parameter_annotation_row : BaseRow
	{
		private UInt32 method_idxI => base.GetValue<UInt32>(0);

		/// <summary>Index into the method_ids list for the identity of the method whose parameters are being annotated.</summary>
		/// <remarks>
		/// Returns the <see cref="method_id_row"/> that identifies which method's parameters are being annotated.
		/// This provides access to the full method definition and metadata.
		/// </remarks>
		public method_id_row method_idx => base.File.METHOD_ID_ITEM[this.method_idxI];

		private UInt32 annotations_offI => base.GetValue<UInt32>(1);

		/// <summary>Offset from the start of the file to the list of annotations for the method parameters.</summary>
		/// <remarks>
		/// Returns the annotation set reference row located at the specified offset from the file start.
		/// The annotation set reference list contains the actual annotations for each parameter of the method.
		/// 
		/// The offset points to a location in the data section and follows the "annotation_set_ref_list" format,
		/// which is an array of <see cref="TableType.ANNOTATION_SET_ITEM"/> offsets, one per method parameter.
		/// </remarks>
		public annotation_set_ref_row annotations_off => base.File.ANNOTATION_SET_REF_LIST.GetRowByOffset(this.annotations_offI);
	}
}