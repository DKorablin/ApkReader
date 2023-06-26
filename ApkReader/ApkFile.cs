using System;
using System.Collections.Generic;
using System.IO;
using AlphaOmega.Debug.Manifest;
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
		private ApkSignature _signatures;
		private AndroidManifest _androidManifest;
		private Boolean _isXmlExists = true;
		private Boolean _isArscExists = true;

		internal Stream ApkStream { get { return _apkStream; } }

		public ApkSignature Signatures
		{
			get { return _signatures ?? (_signatures = new ApkSignature(this)); }
		}

		/// <summary>Raw AndroidManifest.xml</summary>
		public AxmlFile AndroidManifestXml
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

		/// <summary>Check for overal Android Package vaidity</summary>
		/// <remarks>This property will check only for specific files, but not for JAR or APK signatures</remarks>
		public Boolean IsValid
		{
			get
			{
				if(this.AndroidManifestXml != null && this.Resources != null)
					return true;
				foreach(String filePath in this.GetKnownFilesByExtension())//HACK:???
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
				return this._androidManifest ?? (this._androidManifest = AndroidManifest.Load(this.AndroidManifestXml, this.Resources));
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

		/// <summary>GetPackage contents</summary>
		/// <returns></returns>
		public IEnumerable<String> EnumerateFiles()
		{
			foreach(ZipEntry entry in this._apk)
				if(entry.IsFile)
					yield return entry.Name;
		}

		/// <summary>Get header files</summary>
		/// <returns>Header APK files</returns>
		public IEnumerable<String> GetKnownFilesByExtension()
		{
			foreach(String filePath in this.EnumerateFiles())
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

		/// <summary>Получить файл в виде массива байт</summary>
		/// <param name="zipFilePath">Path to the file inside APK</param>
		/// <param name="checkSignature">Check file contents hash with APK Signature</param>
		/// <returns>File contents of null if file not found or hashCheck failed</returns>
		public Byte[] GetFile(String zipFilePath, Boolean checkSignature = false)
		{
			ZipEntry entry = this._apk.GetEntry(zipFilePath);
			if(entry == null)
				return null;

			Byte[] result;
			using(BinaryReader reader = new BinaryReader(this._apk.GetInputStream(entry)))
				result = reader.ReadBytes((Int32)entry.Size);

			if(checkSignature)
			{
				if(this.Signatures.V1SchemeBlock.Manifest == null)
					return null;//MANIFEST.MF file missing
				if(!this.Signatures.V1SchemeBlock.Manifest.ValidateHash(zipFilePath, result))
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