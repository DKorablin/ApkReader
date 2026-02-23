using System;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>Represents an annotation directory item in a DEX file.</summary>
	/// <remarks>
	/// An annotation directory contains all annotations associated with a class, including:
	/// annotations on the class itself, on its fields, on its methods, and on method parameters.
	/// This structure serves as an index to locate all annotations related to a specific class definition.
	/// </remarks>
	public class annotation_directory_row : BaseRow
	{
		/// <summary>Column indices for the annotation directory row structure.</summary>
		internal enum columns
		{
			/// <summary>Offset to the class-level annotations set item</summary>
			class_annotations_off = 0,
			/// <summary>Count of annotated fields</summary>
			fields_size = 1,
			/// <summary>Count of annotated methods</summary>
			annotated_methods_size = 2,
			/// <summary>Count of method parameter annotation lists</summary>
			annotated_parameters_size = 3,
			/// <summary>Field annotation items array</summary>
			field_annotations = 4,
			/// <summary>Method annotation items array</summary>
			method_annotations = 5,
			/// <summary>Parameter annotation items array</summary>
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

		/// <summary>Gets the count of fields annotated by this annotation directory item.</summary>
		public UInt32 fields_size => base.GetValue<UInt32>((UInt16)columns.fields_size);

		/// <summary>Gets the count of methods annotated by this annotation directory item.</summary>
		public UInt32 annotated_methods_size => base.GetValue<UInt32>((UInt16)columns.annotated_methods_size);

		/// <summary>Gets the count of method parameter lists annotated by this annotation directory</summary>
		public UInt32 annotated_parameters_size => base.GetValue<UInt32>((UInt16)columns.annotated_parameters_size);

		private UInt32[] field_annotationsI => base.GetValue<UInt32[]>((UInt16)columns.field_annotations);

		/// <summary>Gets the list of field annotations associated with this class.</summary>
		/// <remarks>
		/// The returned array contains field_annotation_row objects representing annotations
		/// applied to individual fields in the class.
		/// The elements of the list are sorted in increasing order by field_idx.
		/// </remarks>
		public field_annotation_row[] field_annotations
		{
			get
			{
				var table = base.File.field_annotation;
				return Array.ConvertAll(this.field_annotationsI, rowIndex => table[rowIndex]);
			}
		}

		private UInt32[] method_annotationsI => base.GetValue<UInt32[]>((UInt16)columns.method_annotations);

		/// <summary>Gets the list of method annotations associated with this class.</summary>
		/// <remarks>
		/// The returned array contains method_annotation_row objects representing annotations
		/// applied to individual methods in the class.
		/// The elements of the list are sorted in increasing order by method_idx.
		/// </remarks>
		public method_annotation_row[] method_annotations
		{
			get
			{
				var table = base.File.method_annotation;
				return Array.ConvertAll(this.method_annotationsI, rowIndex => table[rowIndex]);
			}
		}

		private UInt32[] parameter_annotationsI => base.GetValue<UInt32[]>((UInt16)columns.parameter_annotations);

		/// <summary>Gets the list of method parameter annotations associated with this class.</summary>
		/// <remarks>
		/// The returned array contains parameter_annotation_row objects representing annotations
		/// applied to individual method parameters in the class.
		/// The elements of the list are sorted in increasing order by method_idx.
		/// </remarks>
		public parameter_annotation_row[] parameter_annotations
		{
			get
			{
				var table = base.File.parameter_annotation;
				return Array.ConvertAll(this.parameter_annotationsI, rowIndex => table[rowIndex]);
			}
		}
	}
}