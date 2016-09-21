using System;
using System.IO;
using System.Linq;
using AlphaOmega.Debug;
using AlphaOmega.Debug.Data;
using AlphaOmega.Debug.Data.Tables;

namespace Demo
{
	class Program
	{
		private const String FilePath = @"C:\Visual Studio Projects\C#\Shared.Classes\AlphaOmega.Debug\FileReader\DexReader\Demo\classes.dex";

		static void Main(string[] args)
		{
			Program.ReadDex(Program.FilePath);
		}

		static void ReadDex(String filePath)
		{
			using(DexFile info = new DexFile(StreamLoader.FromFile(FilePath)))
			{
				Utils.ConsoleWriteMembers(info.header);

				foreach(Dex.map_item mapItem in info.map_list)
					Utils.ConsoleWriteMembers(mapItem);

				foreach(annotation_set_row row in info.ANNOTATION_SET_ITEM)
					Utils.ConsoleWriteMembers(row);

				foreach(annotation_set_ref_row row in info.ANNOTATION_SET_REF_LIST)
					Utils.ConsoleWriteMembers(row);

				foreach(annotation_directory_row row in info.ANNOTATIONS_DIRECTORY_ITEM)
					Utils.ConsoleWriteMembers(row);

				foreach(field_annotation_row row in info.field_annotation)
					Utils.ConsoleWriteMembers(row);

				foreach(parameter_annotation_row row in info.parameter_annotation)
					Utils.ConsoleWriteMembers(row);

				foreach(method_annotation_row row in info.method_annotation)
					Utils.ConsoleWriteMembers(row);

				foreach(class_data_row row in info.CLASS_DATA_ITEM)
					Utils.ConsoleWriteMembers(row);

				foreach(encoded_field_row row in info.encoded_field)
					Utils.ConsoleWriteMembers(row);

				foreach(encoded_method_row row in info.encoded_method)
					Utils.ConsoleWriteMembers(row);

				foreach(code_row row in info.CODE_ITEM)
					Utils.ConsoleWriteMembers(row);

				foreach(try_item_row row in info.try_item)
					Utils.ConsoleWriteMembers(row);

				foreach(encoded_catch_handler_row row in info.encoded_catch_handler_list)
					Utils.ConsoleWriteMembers(row);

				foreach(encoded_type_addr_pair_row row in info.encoded_type_addr_pair)
					Utils.ConsoleWriteMembers(row);

				foreach(type_list_row row in info.TYPE_LIST)
					Utils.ConsoleWriteMembers(row);

				foreach(string_data_row row in info.STRING_DATA_ITEM)
					Utils.ConsoleWriteMembers(row);

				foreach(type_id_row row in info.TYPE_ID_ITEM)
					Utils.ConsoleWriteMembers(row);

				foreach(proto_id_row row in info.PROTO_ID_ITEM)
					Utils.ConsoleWriteMembers(row);

				foreach(field_id_row row in info.FIELD_ID_ITEM)
					Utils.ConsoleWriteMembers(row);

				foreach(method_id_row row in info.METHOD_ID_ITEM)
					Utils.ConsoleWriteMembers(row);

				foreach(class_def_row row in info.CLASS_DEF_ITEM)
					Utils.ConsoleWriteMembers(row);
			}
		}
	}
}