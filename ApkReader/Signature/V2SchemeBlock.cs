using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AlphaOmega.Debug.Signature
{
	/// <summary>Signer information</summary>
	public class V2SchemeBlock
	{
		/// <summary>The digest is stored to decouple signature verification from integrity checking of the APK’s contents</summary>
		/// <remarks>(signature algorithm, digest, signature) tuples</remarks>
		public Dictionary<ApkV2SignatureVerifier.SignatureAlgorithmType, Byte[]> Digests { get; internal set; } = new Dictionary<ApkV2SignatureVerifier.SignatureAlgorithmType, Byte[]>();

		/// <summary>X.509 certificate chain representing the signer’s identity</summary>
		public X509Certificate Certificate { get; internal set; }

		/// <summary>Additional attributes as key-value pairs</summary>
		public Dictionary<ApkV2SignatureVerifier.SignerAttributeId, Byte[]> Attributes { get; internal set; } = new Dictionary<ApkV2SignatureVerifier.SignerAttributeId, Byte[]>();
	}
}