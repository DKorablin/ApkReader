using System;
using System.IO;

namespace Demo
{
	class Program
	{
		private const String ManifestFilePath = @"C:\Visual Studio Projects\C#\Shared.Classes\AlphaOmega.Debug\FileReader\Samples\apk\com.squareenix.dxm.AndroidManifest.xml";
		private const String ResourcesFilePath = @"C:\Visual Studio Projects\C#\Shared.Classes\AlphaOmega.Debug\FileReader\Samples\apk\com.squareenix.dxm.resources.arsc";
		private const String DexFilePath = @"C:\Visual Studio Projects\C#\Shared.Classes\AlphaOmega.Debug\FileReader\Samples\apk\com.askgps.personaltrackerround.classes2.dex";
		private const String ApkFilePath = @"C:\Visual Studio Projects\C#\Shared.Classes\AlphaOmega.Debug\FileReader\Samples\apk";

		static void Main(String[] args)
		{
			ConsoleWriter writer = new ConsoleWriter(false);
			Reader reader = new Reader(writer);
			writer.StartThreadAsync(() =>
			{
				//reader.ReadDex(Program.DexFilePath);
				//reader.ReadManifest(Program.ManifestFilePath);
				//reader.ReadResource(Program.ResourcesFilePath);
				//reader.ReadApkManifest(Program.ManifestFilePath, Program.ResourcesFilePath);

				foreach(String filePath in Directory.GetFiles(ApkFilePath, "*.apk"))
					reader.ReadApk(filePath);
				//reader.ReadAxml(Program.AxmlFilePath);
				Console.WriteLine("FINISHED");
			});
		}
	}
}