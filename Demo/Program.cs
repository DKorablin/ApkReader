using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AlphaOmega.Debug;
using AlphaOmega.Debug.Dex.Tables;
using AlphaOmega.Debug.Manifest;

namespace Demo
{
	class Program
	{
		private const String ManifestFilePath = @"C:\Visual Studio Projects\C#\Shared.Classes\AlphaOmega.Debug\FileReader\Samples\apk\com.squareenix.dxm.AndroidManifest.xml";
		private const String ResourcesFilePath = @"C:\Visual Studio Projects\C#\Shared.Classes\AlphaOmega.Debug\FileReader\Samples\apk\com.squareenix.dxm.resources.arsc";
		private const String DexFilePath = @"C:\Visual Studio Projects\C#\Shared.Classes\AlphaOmega.Debug\FileReader\Samples\apk\com.askgps.personaltrackerround.classes2.dex";
		private const String ApkFilePath = @"C:\Visual Studio Projects\C#\Shared.Classes\AlphaOmega.Debug\FileReader\Samples\apk\com.squareenix.dxm.apk";

		static void Main(String[] args)
		{
			//Program.ReadDex(Program.DexFilePath);
			//Program.ReadManifest(Program.ManifestFilePath);
			//Program.ReadResource(Program.ResourcesFilePath);
			//Program.ReadApkManifest(Program.ManifestFilePath, Program.ResourcesFilePath);
			foreach(String filePath in Directory.GetFiles(Path.GetDirectoryName(ApkFilePath), "*.apk"))
				Program.ReadApk(filePath);
			//Program.ReadAxml(Program.AxmlFilePath);
			Console.ReadLine();
		}

		static void ReadAxml(String filePath)
		{
			using(AxmlFile axml = new AxmlFile(StreamLoader.FromFile(filePath)))
			{
				if(!axml.Header.IsValid)
					throw new ArgumentException("Invalid header");

				Utils.ConsoleWriteMembers(axml.Header);
				Console.WriteLine(axml.RootNode.ConvertToString());
			}
		}

		static void ReadApk(String filePath)
		{
			using(ApkFile apk = new ApkFile(filePath))
			{
				Console.WriteLine("Package: {0}", apk.AndroidManifest.Package);
				Console.WriteLine("Application name: {0} ({1})", apk.AndroidManifest.Application.Label, apk.AndroidManifest.VersionName);

				foreach(String xmlFile in apk.GetHeaderFiles())
					switch(Path.GetExtension(xmlFile).ToLowerInvariant())
					{
					case ".xml":
						/*if(xmlFile.Equals("AndroidManifest.xml", StringComparison.OrdinalIgnoreCase))
							continue;*/

						Byte[] file = apk.GetFile(xmlFile);
						//ManifestFile manifest = new ManifestFile(file);
						//Console.WriteLine(manifest.Xml.ConvertToString());

						AxmlFile axml = new AxmlFile(StreamLoader.FromMemory(file, xmlFile));
						if(axml.Header.IsValid)
						{
							XmlNode xml = axml.RootNode;
							Console.WriteLine("---" + xmlFile + ":");
							Console.WriteLine(xml.ConvertToString());
						} else
						{
							Console.WriteLine("---" + xmlFile + ":");
							Console.WriteLine(System.Text.Encoding.UTF8.GetString(file));
						}
						break;
					}

				ReadApkManifestRecursive(apk.AndroidManifest);
			}
		}

		static void ReadManifest(String filePath)
		{
			AxmlFile manifest = new AxmlFile(StreamLoader.FromFile(filePath));
			Console.Write(manifest.RootNode.ConvertToString());
		}

		static void ReadApkManifest(String manifestPath, String resourcesPath)
		{
			AxmlFile axml = new AxmlFile(StreamLoader.FromFile(manifestPath));
			ArscFile resources = new ArscFile(File.ReadAllBytes(resourcesPath));

			if(!axml.Header.IsValid || !resources.Header.IsValid)
				throw new InvalidOperationException();

			AndroidManifest apk = AndroidManifest.Load(axml, resources);
			Console.WriteLine("Label: " + apk.Application.Label);
			Console.WriteLine("Package: " + apk.Package);
			Console.WriteLine("Icon: " + apk.Application.Icon);
			Console.WriteLine("Permissions: " + String.Join(", ", apk.UsesPermission.Select(p => p.Name)));
			Console.WriteLine("Services: " + String.Join(", ", apk.Application.Service));
			Console.WriteLine("Activities: " + String.Join(", ", apk.Application.Activity));
			Console.WriteLine("Reciever: " + String.Join(", ", apk.Application.Reciever));
			Console.WriteLine("Features: " + String.Join(", ", apk.UsesFeature));
			Console.WriteLine("Uses Libraries: " + String.Join(", ", apk.Application.UsesLibrary.Select(p => p.Name)));
		}

		static void ReadResource(String filePath)
		{
			Byte[] resourceBytes = File.ReadAllBytes(filePath);
			ArscFile resources = new ArscFile(resourceBytes);

			foreach(var item in resources.ResourceMap)
			{
				Console.WriteLine(item.Key + "\t\t\t" + String.Join("; ", item.Value.Select(p => p.Value).ToArray()));
			}

			//OldResourceFile resources2 = new OldResourceFile(resourceBytes);
			//var table2 = resources2.ResourceMap;
			foreach(var item in resources.ResourceMap)
			{
				String key = "@" + item.Key.ToString("X4");
				String value1 = String.Join(";", item.Value.Select(p => p.Value).ToArray());
				//String value2 = String.Join(";", table2[key].ToArray());
				//if(!String.Equals(value1, value2))
				//	throw new Exception("Not equal");
			}
		}

		static void ReadDex(String filePath)
		{
			using(DexFile info = new DexFile(StreamLoader.FromFile(filePath)))
			{
				var items = info.STRING_ID_ITEM;

				foreach(string_data_row row in info.STRING_DATA_ITEM)
					Utils.ConsoleWriteMembers(row);


				Utils.ConsoleWriteMembers(info.header);

				foreach(DexApi.map_item mapItem in info.map_list)
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

		static void ReadApkManifestRecursive(ApkNode parent)
		{
			MethodInfo[] methods = parent.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			foreach(MethodInfo method in methods)
				if(method.GetParameters().Length == 0)
				{
					if(Utils.IsGenericType(method.ReturnType, typeof(IEnumerable<>)))
					{
						Object result = method.Invoke(parent, null);
						foreach(Object item in (System.Collections.IEnumerable)result)
						{
							ApkNode node = (ApkNode)item;
							Utils.ConsoleWriteMembers(node);
							Program.ReadApkManifestRecursive(node);
						}
					} else if(method.ReturnType.BaseType == typeof(ApkNode))
					{
						ApkNode node = (ApkNode)method.Invoke(parent, null);
						Utils.ConsoleWriteMembers(node);
						Program.ReadApkManifestRecursive(node);
					} else if(Utils.IsGenericType(method.ReturnType.BaseType, typeof(ApkNodeT<>)))
					{
						ApkNode node = (ApkNode)method.Invoke(parent, null);
						Utils.ConsoleWriteMembers(node);
						Program.ReadApkManifestRecursive(node);
					}
				}
		}
	}
}