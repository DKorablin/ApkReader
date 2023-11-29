using System;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>banana banana banana</summary>
	public class annotation_directory_row : BaseRow
	{
		internal enum columns
		{
			class_annotations_off = 0,
			fields_size = 1,
			annotated_methods_size = 2,
			annotated_parameters_size = 3,
			field_annotations = 4,
			method_annotations = 5,
			parameter_annotations = 6,
		}

		private UInt32 class_annotations_offI => base.GetValue<UInt32>((UInt16)columns.class_annotations_off);

		/// <summary>
		/// Offset from the start of the file to the annotations made directly on the class, or 0 if the class has no direct annotations.
		/// The offset, if non-zero, should be to a location in the data section.
		/// </summary>
		/// <remarks>The format of the data is specified by "annotation_set_item" below.</remarks>
		public annotation_set_row class_annotations_off
			=> this.class_annotations_offI == 0
				? null
				: base.File.ANNOTATION_SET_ITEM.GetRowByOffset(this.class_annotations_offI);

		/// <summary>Count of fields annotated by this item</summary>
		public UInt32 fields_size => base.GetValue<UInt32>((UInt16)columns.fields_size);

		/// <summary>Count of methods annotated by this item</summary>
		public UInt32 annotated_methods_size => base.GetValue<UInt32>((UInt16)columns.annotated_methods_size);

		/// <summary>Count of method parameter lists annotated by this item</summary>
		public UInt32 annotated_parameters_size => base.GetValue<UInt32>((UInt16)columns.annotated_parameters_size);

		private UInt32[] field_annotationsI => base.GetValue<UInt32[]>((UInt16)columns.field_annotations);

		/// <summary>List of associated field annotations.</summary>
		/// <remarks>The elements of the list must be sorted in increasing order, by field_idx.</remarks>
		public field_annotation_row[] field_annotations
		{
			get
			{
				var table = base.File.field_annotation;
				return Array.ConvertAll(this.field_annotationsI, delegate(UInt32 rowIndex) { return table[rowIndex]; });
			}
		}

		private UInt32[] method_annotationsI => base.GetValue<UInt32[]>((UInt16)columns.method_annotations);

		/// <summary>List of associated method annotations.</summary>
		/// <remarks>The elements of the list must be sorted in increasing order, by method_idx.</remarks>
		public method_annotation_row[] method_annotations
		{
			get
			{
				var table = base.File.method_annotation;
				return Array.ConvertAll(this.method_annotationsI, delegate(UInt32 rowIndex) { return table[rowIndex]; });
			}
		}

		private UInt32[] parameter_annotationsI => base.GetValue<UInt32[]>((UInt16)columns.parameter_annotations);

		/// <summary>List of associated method parameter annotations.</summary>
		/// <remarks>The elements of the list must be sorted in increasing order, by method_idx.</remarks>
		public parameter_annotation_row[] parameter_annotations
		{
			get
			{
				var table = base.File.parameter_annotation;
				return Array.ConvertAll(this.parameter_annotationsI, delegate(UInt32 rowIndex) { return table[rowIndex]; });
			}
		}
	}
}