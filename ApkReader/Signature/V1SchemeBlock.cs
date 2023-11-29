using System;

namespace AlphaOmega.Debug.Signature
{
	/// <summary>APK signing has been a part of Android from the beginning. It is based on signed JAR (v1 scheme)</summary>
	/// <remarks>
	/// v1 signatures do not protect some parts of the APK, such as ZIP metadata.
	/// The APK verifier needs to process lots of untrusted (not yet verified) data structures and then discard data not covered by the signatures.
	/// This offers a sizeable attack surface.
	/// Moreover, the APK verifier must uncompress all compressed entries, consuming more time and memory.
	/// https://docs.oracle.com/javase/8/docs/technotes/guides/jar/jar.html#Signed_JAR_File
	/// </remarks>
	public class V1SchemeBlock
	{
		private readonly ApkFile _apkFile;
		private MfFile _manifest;
		private Boolean _isManifestExists = true;

		/// <summary>META-INF/MANIFEST.MF file reader</summary>
		public MfFile Manifest
		{
			get
			{
				if(this._manifest == null && _isManifestExists)
				{
					Byte[] payload = _apkFile.GetFile("META-INF/MANIFEST.MF");
					if(payload == null)
						_isManifestExists = false;
					else
						this._manifest = new MfFile(payload);
				}
				return this._manifest;

			}
		}

		internal V1SchemeBlock(ApkFile apkFile)
			=> this._apkFile = apkFile;
	}
}