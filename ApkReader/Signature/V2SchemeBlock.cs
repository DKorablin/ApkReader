using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AlphaOmega.Debug.Signature
{
	public class V2SchemeBlock
	{
		public Dictionary<ApkV2SignatureVerifier.SignatureAlgorithmType, Byte[]> Digests { get; internal set; } = new Dictionary<ApkV2SignatureVerifier.SignatureAlgorithmType, Byte[]>();
		public X509Certificate Certificate { get; internal set; }
		public Dictionary<ApkV2SignatureVerifier.SignerAttributeId, Byte[]> Attributes { get; internal set; } = new Dictionary<ApkV2SignatureVerifier.SignerAttributeId, Byte[]>();
	}
}