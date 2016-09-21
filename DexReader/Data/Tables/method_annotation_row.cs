using System;

namespace AlphaOmega.Debug.Data.Tables
{
	/// <summary>banana banana banana</summary>
	public class method_annotation_row : BaseRow
	{
		private UInt32 method_idxI { get { return base.GetValue<UInt32>(0); } }

		/// <summary>Index into the method_ids list for the identity of the method being annotated</summary>
		public method_id_row method_idx { get { return base.File.METHOD_ID_ITEM[this.method_idxI]; } }

		private UInt32 annotations_offI { get { return base.GetValue<UInt32>(1); } }

		/// <summary>Offset from the start of the file to the list of annotations for the method.</summary>
		/// <remarks>
		/// The offset should be to a location in the data section.
		/// The format of the data is specified by "annotation_set_item" below.
		/// </remarks>
		public annotation_set_row annotations_off { get { return base.File.ANNOTATION_SET_ITEM.GetRowByOffset(this.annotations_offI); } }
	}
}