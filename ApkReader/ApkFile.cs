using System;
using System.Collections.Generic;
using System.IO;
using AlphaOmega.Debug.Manifest;
using AlphaOmega.Debug.Signature;

using ICSharpCode.SharpZipLib.Zip;

namespace AlphaOmega.Debug
{
	/// <summary>Android Package description</summary>
	public class ApkFile : IDisposable
	{
		private Stream _apkStream;
		private ZipFile _apk;
		private AxmlFile _xml;
		private ArscFile _res;
		private MfFile _mf;
		private AndroidManifest _androidManifest;
		private Boolean _isXmlExists = true;
		private Boolean _isArscExists = true;
		private Boolean _isMfExists = true;

		/// <summary>Raw AndroidManifest.xml</summary>
		public AxmlFile XmlFile
		{
			get
			{
				if(this._xml == null && _isXmlExists)
				{
					const String FileName = "AndroidManifest.xml";
					Byte[] payload = this.GetFile(FileName);
					if(payload == null)
						_isXmlExists = false;
					else
						this._xml = new AxmlFile(StreamLoader.FromMemory(payload, FileName));
				}
				return this._xml;
			}
		}

		/// <summary>META-INF/MANIFEST.MF file reader</summary>
		public MfFile MfFile
		{
			get
			{
				if(this._mf == null && _isMfExists)
				{
					Byte[] payload = this.GetFile("META-INF/MANIFEST.MF");
					if(payload == null)
						_isMfExists = false;
					else
						this._mf = new MfFile(payload);
				}
				return this._mf;

			}
		}

		/// <summary>Raw Resources.arsc</summary>
		public ArscFile Resources
		{
			get
			{
				if(this._res == null && _isArscExists)
				{
					Byte[] payload = this.GetFile("resources.arsc");
					if(payload == null)
						_isArscExists = false;
					else
						this._res = new ArscFile(payload);
				}
				return this._res;
			}
		}

		/// <summary>Проверка на валидность Android Package</summary>
		public Boolean IsValid
		{
			get
			{
				if(this.XmlFile != null && this.Resources != null)
					return true;
				foreach(String filePath in this.GetHeaderFiles())
					if(Path.GetExtension(filePath).ToLowerInvariant() == ".apk")
						return true;

				return false;
			}
		}

		/// <summary>Android manifest</summary>
		public AndroidManifest AndroidManifest
		{
			get
			{
				return this._androidManifest ?? (this._androidManifest = AndroidManifest.Load(this.XmlFile, this.Resources));
			}
		}

		/// <summary>Specifies a system permission that the user must grant in order for the app to operate correctly</summary>
		public IEnumerable<String> UsesPermission
		{
			get
			{
				if(this.AndroidManifest != null)
					foreach(ApkUsesPermission permission in this.AndroidManifest.UsesPermission)
						yield return permission.Name;
			}
		}

		/// <summary>Specifies a single hardware or software feature used by the application, as a descriptor string</summary>
		public IEnumerable<String> UsesFeature
		{
			get
			{
				if(this.AndroidManifest != null)
					foreach(ApkUsesFeature feature in this.AndroidManifest.UsesFeature)
						yield return feature.Name;
			}
		}

		/// <summary>The name of the class that implements the broadcast receiver, a subclass of BroadcastReceiver</summary>
		public IEnumerable<String> Receiver
		{
			get
			{
				if(this.AndroidManifest != null)
					foreach(ApkReceiver receiver in this.AndroidManifest.Application.Reciever)
						yield return receiver.Name;
			}
		}

		/// <summary>The name of the class that implements the content provider, a subclass of ContentProvider</summary>
		public IEnumerable<String> Provider
		{
			get
			{
				if(this.AndroidManifest != null)
					foreach(ApkProvider provider in this.AndroidManifest.Application.Provider)
						yield return provider.Name;
			}
		}

		/// <summary>The name of the Service subclass that implements the service</summary>
		public IEnumerable<String> Service
		{
			get
			{
				if(this.AndroidManifest != null)
					foreach(ApkService service in this.AndroidManifest.Application.Service)
						yield return service.Name;
			}
		}

		/// <summary>Create instance of android package description</summary>
		/// <param name="filePath">Physical file path</param>
		public ApkFile(String filePath)
			: this(new FileStream(filePath, FileMode.Open, FileAccess.Read))
		{
		}

		/// <summary>Create instance of android package desctiption</summary>
		/// <param name="buffer">Raw file bytes</param>
		public ApkFile(Byte[] buffer)
			: this(new MemoryStream(buffer))
		{
		}

		/// <summary>Create instance of android package desctiption</summary>
		/// <param name="stream">File stream</param>
		public ApkFile(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException(nameof(stream));

			this._apkStream = stream;
			this._apk = new ZipFile(this._apkStream);
		}

		public IEnumerable<ApkSignatureInfo> GetApkSignatures()
		{
			ApkSignature signatures = new ApkSignature(this._apkStream);
			return signatures.Blocks;
		}

		/// <summary>GetPackage contents</summary>
		/// <returns></returns>
		public IEnumerable<String> GetFiles()
		{
			foreach(ZipEntry entry in this._apk)
				if(entry.IsFile)
					yield return entry.Name;
		}

		/// <summary>Get header files</summary>
		/// <returns>Header APK files</returns>
		public IEnumerable<String> GetHeaderFiles()
		{
			foreach(String filePath in this.GetFiles())
				switch(Path.GetExtension(filePath).ToLowerInvariant())
				{
				case ".apk":
				case ".xapk":
				case ".dex":
				case ".arsc":
					yield return filePath;
					break;
				case ".xml":
					yield return filePath;
					break;
				}
		}

		/// <summary>Получить файл в виде потока байт</summary>
		/// <param name="zipFilePath">Путь к файлу в архиве</param>
		/// <param name="checkHash">Проверить корректность Hash из MANIFEST.MF</param>
		/// <returns>Поток файла из архива</returns>
		public Stream GetFileStream(String zipFilePath, Boolean checkHash = false)
		{
			ZipEntry entry = this._apk.GetEntry(zipFilePath);
			if(entry == null)
				return null;

			Stream result = this._apk.GetInputStream(entry);

			if(checkHash)
			{
				if(this.MfFile == null)
					return null;//MANIFEST.MF file missing
				if(!this.MfFile.ValidateHash(zipFilePath, result, result.Length))
					return null;//Hash validation failed
				result.Position = 0;//Reset stream position
			}
			return result;
		}

		/// <summary>Получить файл в виде массива байт</summary>
		/// <param name="zipFilePath">Path to the file inside APK</param>
		/// <param name="checkHash">Check file contents hash with MANIFEST.MF file</param>
		/// <returns>File contents of null if file not found or hashCheck failed</returns>
		public Byte[] GetFile(String zipFilePath, Boolean checkHash = false)
		{
			ZipEntry entry = this._apk.GetEntry(zipFilePath);
			if(entry == null)
				return null;

			Byte[] result;
			using(BinaryReader reader = new BinaryReader(this._apk.GetInputStream(entry)))
				result = reader.ReadBytes((Int32)entry.Size);

			if(checkHash)
			{
				if(this.MfFile == null)
					return null;//MANIFEST.MF file missing
				if(!this.MfFile.ValidateHash(zipFilePath, result))
					return null;//Hash validation failed
			}
			return result;
		}

		/// <summary>Clears base apk file</summary>
		public void Dispose()
		{
			if(this._apk != null)
			{
				this._apk.Close();
				this._apk = null;
			}
		}
	}
}