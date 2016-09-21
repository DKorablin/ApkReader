using System;
using AlphaOmega.Debug.Data.Tables;

namespace AlphaOmega.Debug.Data
{
	internal class TableAnnotationDirectory : Table
	{
		internal TableAnnotationDirectory()
			: base(TableType.ANNOTATIONS_DIRECTORY_ITEM)
		{ }

		protected internal override Cell ReadCellPayload(DexFile file, Column column, Cell[] cells, ref UInt32 offset)
		{
			Cell result;
			switch((annotation_directory_row.columns)column.Index)
			{
			case annotation_directory_row.columns.field_annotations:
				UInt32 fields_size = cells[(UInt16)annotation_directory_row.columns.fields_size].RawValue;
				UInt32[] field_annotations = file.GetSectionTable(TableType.field_annotation).ReadInnerSection(file, ref offset, fields_size);
				result = new Cell(column, fields_size, field_annotations);
				break;
			case annotation_directory_row.columns.method_annotations:
				UInt32 annotated_methods_size = cells[(UInt16)annotation_directory_row.columns.annotated_methods_size].RawValue;
				UInt32[] method_annotations = file.GetSectionTable(TableType.method_annotation).ReadInnerSection(file, ref offset, annotated_methods_size);
				result = new Cell(column, annotated_methods_size, method_annotations);
				break;
			case annotation_directory_row.columns.parameter_annotations:
				UInt32 annotated_parameters_size = cells[(UInt16)annotation_directory_row.columns.annotated_parameters_size].RawValue;
				UInt32[] parameter_annotations = file.GetSectionTable(TableType.parameter_annotation).ReadInnerSection(file, ref offset, annotated_parameters_size);
				result = new Cell(column, annotated_parameters_size, parameter_annotations);
				break;
			default:
				result = base.ReadCellPayload(file, column, cells, ref offset);
				break;
			}
			return result;
		}
	}
}