using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace AlphaOmega.Debug.Signature
{
	/// <summary>
	/// APK Signature Scheme v2 is a whole-file signature scheme that increases verification speed and strengthens integrity guarantees by detecting any changes to the protected parts of the APK. 
	/// </summary>
	public class ApkV2SignatureVerifier : ApkSignatureVerifier
	{
		/// <summary>Type of V2 signature algorithm is used for verification</summary>
		public enum SignatureAlgorithmType : UInt32
		{
			/// <summary>RSASSA-PSS with SHA2-256 digest, SHA2-256 MGF1, 32 bytes of salt, trailer: 0xbc, content digested using SHA2-256 in 1 MB chunks</summary>
			RSA_PSS_WITH_SHA256 = 0x0101,
			/// <summary>RSASSA-PSS with SHA2-512 digest, SHA2-512 MGF1, 64 bytes of salt, trailer: 0xbc, content digested using SHA2-512 in 1 MB chunks</summary>
			RSA_PSS_WITH_SHA512 = 0x0102,
			/// <summary>RSASSA-PKCS1-v1_5 with SHA2-256 digest, content digested using SHA2-256 in 1 MB chunks</summary>
			RSA_PKCS1_V1_5_WITH_SHA256 = 0x0103,
			/// <summary>RSASSA-PKCS1-v1_5 with SHA2-512 digest, content digested using SHA2-512 in 1 MB chunks</summary>
			RSA_PKCS1_V1_5_WITH_SHA512 = 0x0104,
			/// <summary>ECDSA with SHA2-256 digest, content digested using SHA2-256 in 1 MB chunks</summary>
			ECDSA_WITH_SHA256 = 0x0201,
			/// <summary>ECDSA with SHA2-512 digest, content digested using SHA2-512 in 1 MB chunks</summary>
			ECDSA_WITH_SHA512 = 0x0202,
			/// <summary>DSA with SHA2-256 digest, content digested using SHA2-256 in 1 MB chunks</summary>
			DSA_WITH_SHA256 = 0x0301,
			/// <summary>RSASSA-PKCS1-v1_5 with SHA2-256 digest, content digested using SHA2-256 in 4 KB chunks, in the same way fsverity operates</summary>
			/// <remarks>This digest and the content length (before digestion, 8 bytes in little endian) construct the final digest</remarks>
			VERITY_RSA_PKCS1_V1_5_WITH_SHA256 = 0x0421,
			/// <summary>ECDSA with SHA2-256 digest, content digested using SHA2-256 in 4 KB chunks, in the same way fsverity operates</summary>
			/// <remarks>This digest and the content length (before digestion, 8 bytes in little endian) construct the final digest</remarks>
			VERITY_ECDSA_WITH_SHA256 = 0x0423,
			/// <summary>DSA with SHA2-256 digest, content digested using SHA2-256 in 4 KB chunks, in the same way fsverity operates</summary>
			/// <remarks>This digest and the content length (before digestion, 8 bytes in little endian) construct the final digest</remarks>
			VERITY_DSA_WITH_SHA256 = 0x0425,
		}

		/// <summary>Known additional attribute identifiers</summary>
		public enum SignerAttributeId : UInt32
		{
			/// <summary>Attribute to check whether a newer APK Signature Scheme signature was stripped</summary>
			STRIPPING_PROTECTION_ATTR_ID = 0xbeeff00d,
		}

		/// <summary>Known additional attribute values</summary>
		public enum SignerAttributeValue : Int32
		{
			/// <summary>ID of this signature scheme as used in X-Android-APK-Signed header used in JAR signing</summary>
			SF_ATTRIBUTE_ANDROID_APK_SIGNED_ID = 2,
		}

		internal ApkV2SignatureVerifier(Byte[] data)
			: base(ApkSignatureVerifier.BlockId.APK_SIGNATURE_SCHEME_V2_BLOCK_ID, data)
		{ }

		/// <summary>Gets signing certificate that is stored inside APK Signature V2 Block</summary>
		/// <returns>x509 certificate</returns>
		public V2SchemeBlock GetSigningCertificate()
		{
			V2SchemeBlock result = new V2SchemeBlock();
			using(MemoryStream stream = new MemoryStream(base.Data))
			using(BinaryReader reader = new BinaryReader(stream))
			{
				Int32 signerLength = reader.ReadInt32();
				Int32 signedDataLength = reader.ReadInt32();
				UInt32 unknown1 = reader.ReadUInt32();

				//length - prefixed sequence of length - prefixed digests
				foreach(BinaryReader slice in getLengthPrefixedSlice(reader))
				{
					SignatureAlgorithmType algorithm = (SignatureAlgorithmType)slice.ReadInt32();
					Int32 digestArrayLength = slice.ReadInt32();
					Byte[] digest = slice.ReadBytes(digestArrayLength);
					result.Digests.Add(algorithm,digest);
				}

				//length-prefixed sequence of X.509 certificates
				foreach(BinaryReader slice in getLengthPrefixedSlice(reader))
				{
					Byte[] certData = slice.ReadBytes((Int32)slice.BaseStream.Length);
					result.Certificate = new X509Certificate(certData);
				}

				//length-prefixed sequence of length-prefixed additional attributes
				foreach(BinaryReader slice in getLengthPrefixedSlice(reader))
				{
					SignerAttributeId attributeId = (SignerAttributeId)slice.ReadUInt32();
					Int32 valueLength = Utils.Remaining(slice);
					Byte[] attributeValue = slice.ReadBytes(valueLength);
					result.Attributes.Add(attributeId, attributeValue);

					switch(attributeId)
					{
					case SignerAttributeId.STRIPPING_PROTECTION_ATTR_ID:
						if(attributeValue.Length < sizeof(Int32))
							throw new IOException($"V2 Signature Scheme Stripping Protection Attribute value too small. Expected 4 bytes, but found {valueLength}");

						SignerAttributeValue vers = (SignerAttributeValue)BitConverter.ToInt32(attributeValue, 0);
						if(vers == SignerAttributeValue.SF_ATTRIBUTE_ANDROID_APK_SIGNED_ID)
							throw new ArgumentException("V2 signature indicates APK is signed using APK Signature Scheme v3, but none was found. Signature stripped?");
						break;
					default://Unknown attribute skipping...
						break;
					}
				}

				//length-prefixed sequence of length-prefixed signatures
				foreach(BinaryReader slice in getLengthPrefixedSlice(reader))
				{
					SignatureAlgorithmType algorithmType = (SignatureAlgorithmType)slice.ReadInt32();
					Int32 signatureLength = slice.ReadInt32();
					Byte[] signedData = slice.ReadBytes(signatureLength);
				}

				Int32 publicKeyLength = reader.ReadInt32();
				Byte[] publicKey = reader.ReadBytes(publicKeyLength);
			}
			return result;
		}

		private static IEnumerable<BinaryReader> getLengthPrefixedSlice(BinaryReader reader)
		{
			if(Utils.Remaining(reader) < sizeof(Int32))
				throw new IOException("Remaining buffer too short to contain length of length-prefixed field.");

			Int32 allSliceLength = reader.ReadInt32();
			Int32 allSlicePositionEnd = (Int32)reader.BaseStream.Position + allSliceLength;
			if(allSliceLength < 0)
				throw new ArgumentException("Negative length");

			while(reader.BaseStream.Position < allSlicePositionEnd)
			{
				Int32 sliceLength = reader.ReadInt32();
				if(sliceLength > allSlicePositionEnd)
					throw new IOException("Length-prefixed field longer than remaining buffer.");

				Byte[] slice = reader.ReadBytes(sliceLength);
				yield return new BinaryReader(new MemoryStream(slice));
			}
		}
	}
}