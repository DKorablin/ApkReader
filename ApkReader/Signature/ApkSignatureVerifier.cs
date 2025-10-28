using System;

namespace AlphaOmega.Debug.Signature
{
	/// <summary>
	/// Application signing allows developers to identify the author of the application and to update their application without creating complicated interfaces and permissions.
	/// Every application that is run on the Android platform must be signed by the developer.
	/// </summary>
	public class ApkSignatureVerifier
	{
		/// <summary>Known signature scheme block ID</summary>
		public enum BlockId : UInt32
		{
			/// <summary>
			/// APK Signature Scheme v2 is a whole-file signature scheme that increases verification speed and strengthens integrity guarantees by detecting any changes to the protected parts of the APK. 
			/// </summary>
			APK_SIGNATURE_SCHEME_V2_BLOCK_ID = 0x7109871a,
			/// <summary>
			/// The v3 scheme is designed to be very similar to the v2 scheme.
			/// It has the same general format and supports the same signature algorithm IDs, key sizes, and EC curves.
			/// However, the v3 scheme adds information about the supported SDK versions and the proof-of-rotation struct. 
			/// </summary>
			APK_SIGNATURE_SCHEME_V3_BLOCK_ID = 0xf05368c0,
			/// <summary>
			/// Android 13 supports APK Signature Scheme v3.1, an improvement on the existing APK Signature Scheme v3.
			/// The v3.1 scheme addresses some of the known issues with APK Signature Scheme v3 regarding rotation.
			/// In particular, the v3.1 signature scheme supports SDK version targeting, which allows rotation to target a later release of the platform.
			/// </summary>
			APK_SIGNATURE_SCHEME_V31_BLOCK_ID = 0x1b93ad61,
			/// <summary>Signing block Id for SDK dependency block</summary>
			DEPENDENCY_INFO_BLOCK_ID = 0x504b4453,
			/// <summary>Verity padding block</summary>
			VERITY_PADDING_BLOCK_ID = 0x42726577,
		}

		/// <summary>Signature block id</summary>
		public BlockId Id { get; }

		/// <summary>Block data</summary>
		public Byte[] Data { get; }

		internal ApkSignatureVerifier(ApkSignatureVerifier.BlockId id, Byte[] data)
		{
			this.Id = id;
			this.Data = data;
		}
	}
}
