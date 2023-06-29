﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using AlphaOmega.Debug.Signature;
using static AlphaOmega.Debug.ApkSignature.Api;

namespace AlphaOmega.Debug
{
	/// <summary>
	/// Application signing allows developers to identify the author of the application and to update their application without creating complicated interfaces and permissions.
	/// Every application that is run on the Android platform must be signed by the developer.
	/// </summary>
	/// <remarks>https://source.android.com/docs/security/features/apksigning</remarks>
	public class ApkSignature : IEnumerable<ApkSignatureVerifier>
	{
		internal sealed class Api
		{
			[StructLayout(LayoutKind.Sequential, Pack = 2)]
			public struct EndOfCentralDirectoryFileHeader
			{
				/// <summary>EoF central directory signature value</summary>
				public const Int32 SignatureValue = 0x06054b50;
				/// <summary>End of central directory signature = 0x06054b50</summary>
				public Int32 signature;
				/// <summary>Number of this disk (or 0xffff for ZIP64)</summary>
				public Int16 diskCount;
				/// <summary>Disk where central directory starts (or 0xffff for ZIP64)</summary>
				public Int16 diskEntryStartOffset;
				/// <summary>Number of central directory records on this disk (or 0xffff for ZIP64)</summary>
				public Int16 diskEntries;
				/// <summary>Total number of central directory records (or 0xffff for ZIP64)</summary>
				public Int16 totalEntries;
				/// <summary>Size of central directory (bytes) (or 0xffffffff for ZIP64)</summary>
				public Int32 centralDirectorySize;
				/// <summary>Offset of start of central directory, relative to start of archive (or 0xffffffff for ZIP64)</summary>
				public Int32 offsetToCentralDirectory;
				/// <summary>Comment length (n)</summary>
				public Int16 commentsLength;

				/// <summary>Check header for validation</summary>
				public Boolean IsValid { get { return signature == SignatureValue; } }
			}

			[StructLayout(LayoutKind.Sequential, Pack = 2)]
			public struct ApkSignatureSchemeHeader
			{
				private const Int64 APK_SIG_BLOCK_MAGIC_LO = 0x20676953204b5041L;
				private const Int64 APK_SIG_BLOCK_MAGIC_HI = 0x3234206b636f6c42L;
				/// <summary>Size of all signature schemes blocks</summary>
				public UInt64 sizeOfScheme;

				/// <summary>Validation value</summary>
				[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
				public Byte[] magic;

				/// <summary>Converted magic value to string</summary>
				/// <remarks>This value must be equals to: "APK Sig Block 42" string</remarks>
				public String MagicStr { get { return System.Text.Encoding.ASCII.GetString(magic); } }

				/// <summary>Check header for validation</summary>
				public Boolean IsValid
				{
					get
					{
						return BitConverter.ToInt64(magic, 0) == APK_SIG_BLOCK_MAGIC_LO
							&& BitConverter.ToInt64(magic, sizeof(Int64)) == APK_SIG_BLOCK_MAGIC_HI;
					}
				}
			}
		}

		private readonly ApkFile _apk;
		private V1SchemeBlock _v1Block;
		private Dictionary<ApkSignatureVerifier.BlockId, ApkSignatureVerifier> _blocks = new Dictionary<ApkSignatureVerifier.BlockId, ApkSignatureVerifier>();

		private Dictionary<ApkSignatureVerifier.BlockId, ApkSignatureVerifier> Blocks
		{
			get
			{
				return _blocks ?? (_blocks = ReadSignatures(this._apk.ApkStream));
			}
		}

		/// <summary>Package contains signature blocks</summary>
		public Boolean IsEmpty { get { return this.Blocks.Count > 0; } }

		/// <summary>JAR signature</summary>
		public V1SchemeBlock V1SchemeBlock
		{
			get
			{
				return _v1Block ?? (_v1Block = new V1SchemeBlock(_apk));
			}
		}

		/// <summary>Get signature block by block ID or null if block is not found</summary>
		/// <param name="id">Block identifier for required schema</param>
		/// <returns>Strongly typed APK signature block</returns>
		public ApkSignatureVerifier this[ApkSignatureVerifier.BlockId id]
		{
			get
			{
				ApkSignatureVerifier result = null;
				return this.Blocks.TryGetValue(id, out result) ? result : (ApkSignatureVerifier)null;
			}
		}

		/// <summary>Try to get signature block by strongly typed block wrapper</summary>
		/// <typeparam name="T">Strongly typed block wrapper type</typeparam>
		/// <returns>Found strongly typed signature block</returns>
		/// <exception cref="NotSupportedException">Only V2 signature block supported</exception>
		public T GetBlockByType<T>() where T : ApkSignatureVerifier
		{
			if(typeof(T) == typeof(ApkV2SignatureVerifier))
			{
				ApkSignatureVerifier result;
				return this.Blocks.TryGetValue(ApkSignatureVerifier.BlockId.APK_SIGNATURE_SCHEME_V2_BLOCK_ID, out result)
					? (T)result : (T)null;
			} else
				throw new NotImplementedException();
		}

		/// <summary>Get all found signature blocks</summary>
		/// <returns>List of signature blocks</returns>
		public IEnumerator<ApkSignatureVerifier> GetEnumerator()
		{
			return this.Blocks.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal ApkSignature(ApkFile apk)
		{
			_apk = apk;
		}

		/// <summary>Read all signature blocks from APK file</summary>
		/// <remarks>This method will not validate package for integrity but only read apropriate structures from binary file</remarks>
		/// <param name="stream">Stream to APK file</param>
		/// <returns>Stream of signature block IDs and known singature verifier object</returns>
		/// <exception cref="ArgumentNullException">Stream is null</exception>
		/// <exception cref="ArgumentException">Stream is read only</exception>
		public static Dictionary<ApkSignatureVerifier.BlockId, ApkSignatureVerifier> ReadSignatures(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException(nameof(stream));
			if(stream.CanSeek == false || stream.CanRead == false)
				throw new ArgumentException("stream is readonly", nameof(stream));

			Dictionary<ApkSignatureVerifier.BlockId, ApkSignatureVerifier> result = new Dictionary<ApkSignatureVerifier.BlockId, ApkSignatureVerifier>();

			BinaryReader br = new BinaryReader(stream);//Using will close underlying stream, considering that we didn’t open it, it’s not up to us to close it

			//APK Signature Scheme v2 verification: ZIP End of Central Directory is not followed by more data
			br.BaseStream.Position = br.BaseStream.Length - Marshal.SizeOf(typeof(EndOfCentralDirectoryFileHeader));
			EndOfCentralDirectoryFileHeader eocd = Utils.PtrToStructure<EndOfCentralDirectoryFileHeader>(br);
			if(eocd.IsValid == false)
				return result;

			br.BaseStream.Position = eocd.offsetToCentralDirectory;

			br.BaseStream.Seek(-Marshal.SizeOf(typeof(ApkSignatureSchemeHeader)), SeekOrigin.Current);
			Int64 apkSignatureBlockEnd = br.BaseStream.Position;
			ApkSignatureSchemeHeader signatureHeader = Utils.PtrToStructure<ApkSignatureSchemeHeader>(br);
			if(signatureHeader.IsValid == false)
				return result;

			Int64 apkSignatureSchemeStart = (Int64)eocd.offsetToCentralDirectory - (Int64)signatureHeader.sizeOfScheme;

			br.BaseStream.Position = apkSignatureSchemeStart;
			while(br.BaseStream.Position < apkSignatureBlockEnd)
			{
				Int64 sizeOfBlock2 = br.ReadInt64();
				if(sizeOfBlock2 < sizeof(UInt32) || sizeOfBlock2 > apkSignatureBlockEnd)
					return result;//"APK Signing Block entry #"+_blocks.Length+" size out of range: " + sizeOfBlock2

				ApkSignatureVerifier.BlockId id = (ApkSignatureVerifier.BlockId)br.ReadUInt32();
				Byte[] blockData = br.ReadBytes((Int32)(sizeOfBlock2 - sizeof(UInt32)));

				ApkSignatureVerifier signature;
				switch(id)
				{
				case ApkSignatureVerifier.BlockId.APK_SIGNATURE_SCHEME_V2_BLOCK_ID:
					signature = new ApkV2SignatureVerifier(blockData);
					break;
				default:
					signature = new ApkSignatureVerifier(id, blockData);
					break;
				}
				result.Add(id, signature);
			}

			return result;
		}
	}
}