using System;
using AlphaOmega.Debug.Dex;

namespace AlphaOmega.Debug.Dex
{
	internal struct Constant
	{
		public static Column[] GetTableColumns(TableType type)
		{
			ColumnType[] columnTypes;
			String[] columnNames;

			switch(type)
			{
			case TableType.STRING_ID_ITEM:
				columnTypes = new ColumnType[] { ColumnType.UInt32, };
				columnNames = new String[] { "string_data_off", };
				break;
			case TableType.STRING_DATA_ITEM:
				columnTypes = new ColumnType[] { ColumnType.String, };
				columnNames = new String[] { "data", };
				break;
			case TableType.TYPE_ID_ITEM:
				columnTypes = new ColumnType[] { ColumnType.UInt32, };
				columnNames = new String[] { "descriptor_idx", };
				break;
			case TableType.PROTO_ID_ITEM:
				columnTypes = new ColumnType[] { ColumnType.UInt32, ColumnType.UInt32, ColumnType.UInt32, };
				columnNames = new String[] { "shorty_idx", "return_type_idx", "parameters_off", };
				break;
			case TableType.FIELD_ID_ITEM:
				columnTypes = new ColumnType[] { ColumnType.UInt16, ColumnType.UInt16, ColumnType.UInt32, };
				columnNames = new String[] { "class_idx", "type_idx", "name_idx", };
				break;
			case TableType.METHOD_ID_ITEM:
				columnTypes = new ColumnType[] { ColumnType.UInt16, ColumnType.UInt16, ColumnType.UInt32, };
				columnNames = new String[] { "class_idx", "proto_idx", "name_idx", };
				break;
			case TableType.CLASS_DEF_ITEM:
				columnTypes = new ColumnType[] { ColumnType.UInt32, ColumnType.UInt32, ColumnType.UInt32, ColumnType.UInt32, ColumnType.UInt32, ColumnType.UInt32, ColumnType.UInt32, ColumnType.UInt32, };
				columnNames = new String[] { "class_idx", "access_flags", "superclass_idx", "interfaces_off", "source_file_idx", "annotations_off", "class_data_off", "static_values_off", };
				break;
			case TableType.TYPE_LIST:
				columnTypes = new ColumnType[] { ColumnType.UInt16Array, };
				columnNames = new String[] { "type_idx", };
				break;
			case TableType.ANNOTATION_SET_REF_LIST:
				columnTypes = new ColumnType[] { ColumnType.UInt32Array, };
				columnNames = new String[] { "annotations_off", };
				break;
			case TableType.ANNOTATION_SET_ITEM:
				columnTypes = new ColumnType[] { ColumnType.UInt32Array, };
				columnNames = new String[] { "annotation_off", };
				break;
			case TableType.ANNOTATIONS_DIRECTORY_ITEM:
				columnTypes = new ColumnType[] { ColumnType.UInt32, ColumnType.UInt32, ColumnType.UInt32, ColumnType.UInt32, ColumnType.Payload, ColumnType.Payload, ColumnType.Payload, };
				columnNames = new String[] { "class_annotations_off", "fields_size", "annotated_methods_size", "annotated_parameters_size", "field_annotations", "method_annotations", "parameter_annotations", };
				break;
			case TableType.field_annotation:
				columnTypes = new ColumnType[] { ColumnType.UInt32, ColumnType.UInt32, };
				columnNames = new String[] { "field_idx", "annotations_off", };
				break;
			case TableType.method_annotation:
				columnTypes = new ColumnType[] { ColumnType.UInt32, ColumnType.UInt32, };
				columnNames = new String[] { "method_idx", "annotations_off", };
				break;
			case TableType.parameter_annotation:
				columnTypes = new ColumnType[] { ColumnType.UInt32, ColumnType.UInt32, };
				columnNames = new String[] { "method_idx", "annotations_off", };
				break;
			case TableType.CLASS_DATA_ITEM:
				columnTypes = new ColumnType[] { ColumnType.ULeb128, ColumnType.ULeb128, ColumnType.ULeb128, ColumnType.ULeb128, ColumnType.Payload, ColumnType.Payload, ColumnType.Payload, ColumnType.Payload, };
				columnNames = new String[] { "static_fields_size", "instance_fields_size", "direct_methods_size", "virtual_methods_size", "static_fields", "instance_fields", "direct_methods", "virtual_methods", };
				break;
			case TableType.encoded_field:
				columnTypes = new ColumnType[] { ColumnType.ULeb128, ColumnType.ULeb128, };
				columnNames = new String[] { "field_idx_diff", "access_flags", };
				break;
			case TableType.encoded_method:
				columnTypes = new ColumnType[] { ColumnType.ULeb128, ColumnType.ULeb128, ColumnType.ULeb128, };
				columnNames = new String[] { "method_idx_diff", "access_flags", "code_off", };
				break;
			case TableType.CODE_ITEM:
				columnTypes = new ColumnType[] { ColumnType.UInt16, ColumnType.UInt16, ColumnType.UInt16, ColumnType.UInt16, ColumnType.UInt32, ColumnType.UInt16Array, ColumnType.Payload, ColumnType.Payload, };
				columnNames = new String[] { "registers_size", "ins_size", "outs_size", "tries_size", "debug_info_off", "insns", "tries", "handlers", };
				break;
			case TableType.try_item:
				columnTypes = new ColumnType[] { ColumnType.UInt32, ColumnType.UInt16, ColumnType.UInt16, };
				columnNames = new String[] { "start_addr", "insn_count", "handler_off", };
				break;
			case TableType.encoded_catch_handler_list:
				columnTypes = new ColumnType[] { ColumnType.SLeb128, ColumnType.Payload, ColumnType.Payload, };
				columnNames = new String[] { "size", "handlers", "catch_all_addr", };
				break;
			case TableType.encoded_type_addr_pair:
				columnTypes = new ColumnType[] { ColumnType.ULeb128, ColumnType.ULeb128, };
				columnNames = new String[] { "type_idx", "addr", };
				break;
			default:
				throw new NotImplementedException(String.Format("Type {0} not implemented", type));
			}

			if(columnTypes.Length != columnNames.Length)
				throw new InvalidOperationException("Length of column type and names must be equal");

			Column[] result = new Column[columnTypes.Length];
			for(UInt16 loop = 0; loop < result.Length; loop++)
				result[loop] = new Column(loop, columnNames[loop], columnTypes[loop]);

			return result;
		}
	}
}