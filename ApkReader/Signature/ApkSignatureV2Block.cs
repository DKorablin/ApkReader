using System;
using System.Security.Cryptography.X509Certificates;

namespace AlphaOmega.Debug.Signature
{
	/// <summary>
	/// APK Signature Scheme v2 is a whole-file signature scheme that increases verification speed and strengthens integrity guarantees by detecting any changes to the protected parts of the APK. 
	/// </summary>
	public class ApkSignatureV2Block : ApkSignatureBlock
	{
		internal ApkSignatureV2Block(Byte[] data)
			: base(ApkSignatureBlock.BlockId.APK_SIGNATURE_SCHEME_V2_BLOCK_ID, data)
		{

		}

		/// <summary>Gets signing certificate that is stored inside APK Signature V2 Block</summary>
		/// <returns>x509 certificate</returns>
		public X509Certificate GetSigningCertificate()
		{
			const Int32 position = 0x78;//TODO: Unknown data stored
			Byte[] certData = new Byte[base.Data.Length - position];
			Array.Copy(base.Data, position, certData, 0, certData.Length);

			return new X509Certificate(certData);
		}
	}
}