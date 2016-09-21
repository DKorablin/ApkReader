using System;
using AlphaOmega.Debug.Data.Tables;

namespace AlphaOmega.Debug.Data
{
	internal class TableClass : Table
	{
		internal TableClass()
			: base(TableType.CLASS_DATA_ITEM)
		{ }

		protected internal override Cell ReadCellPayload(DexFile file, Column column, Cell[] cells, ref UInt32 offset)
		{
			Cell result;
			switch((class_data_row.columns)column.Index)
			{
			case class_data_row.columns.static_fields:
				UInt32 static_fields_size = cells[(UInt32)class_data_row.columns.static_fields_size].RawValue;
				UInt32[] static_fields = file.GetSectionTable(TableType.encoded_field).ReadInnerSection(file, ref offset, static_fields_size);
				result = new Cell(column, static_fields_size, static_fields);
				break;
			case class_data_row.columns.instance_fields:
				UInt32 instance_fields_size = cells[(UInt32)class_data_row.columns.instance_fields_size].RawValue;
				UInt32[] instance_fields = file.GetSectionTable(TableType.encoded_field).ReadInnerSection(file, ref offset, instance_fields_size);
				result = new Cell(column, instance_fields_size, instance_fields);
				break;
			case class_data_row.columns.direct_methods:
				UInt32 direct_methods_size = cells[(UInt32)class_data_row.columns.direct_methods_size].RawValue;
				UInt32[] direct_methods = file.GetSectionTable(TableType.encoded_method).ReadInnerSection(file, ref offset, direct_methods_size);
				result = new Cell(column, direct_methods_size, direct_methods);
				break;
			case class_data_row.columns.virtual_methods:
				UInt32 virtual_methods_size = cells[(UInt32)class_data_row.columns.virtual_methods_size].RawValue;
				UInt32[] virtual_methods = file.GetSectionTable(TableType.encoded_method).ReadInnerSection(file, ref offset, virtual_methods_size);
				result = new Cell(column, virtual_methods_size, virtual_methods);
				break;
			default:
				result = base.ReadCellPayload(file, column, cells, ref offset);
				break;
			}

			return result;
		}
	}
}