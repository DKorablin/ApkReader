using System;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>banana banana banana</summary>
	public class field_annotation_row : BaseRow
	{
		private UInt32 field_idxI => base.GetValue<UInt32>(0);

		/// <summary>Index into the field_ids list for the identity of the field being annotated</summary>
		public field_id_row field_idx => base.File.FIELD_ID_ITEM[this.field_idxI];

		private UInt32 annotations_offI => base.GetValue<UInt32>(1);

		/// <summary>Offset from the start of the file to the list of annotations for the field.</summary>
		/// <remarks>
		/// The offset should be to a location in the data section.
		/// The format of the data is specified by "annotation_set_item" below.
		/// </remarks>
		public annotation_set_row annotations_off => base.File.ANNOTATION_SET_ITEM.GetRowByOffset(this.annotations_offI);
	}
}