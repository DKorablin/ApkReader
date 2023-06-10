using System;
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
	public class ApkSignature : IEnumerable<ApkSignatureBlock>
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
				/// <summary>Size of all signature schemes blocks</summary>
				public UInt64 sizeOfScheme;
				/// <summary>Validation value</summary>
				[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
				public Byte[] magic;
				/// <summary>Converted magic value to string</summary>
				public String MagicStr { get { return System.Text.Encoding.ASCII.GetString(magic); } }
				/// <summary>Check header for validation</summary>
				public Boolean IsValid { get { return MagicStr == "APK Sig Block 42"; } }
			}
		}

		private Dictionary<ApkSignatureBlock.BlockId, ApkSignatureBlock> _blocks = new Dictionary<ApkSignatureBlock.BlockId, ApkSignatureBlock>();

		/// <summary>Package contains signature blocks</summary>
		public Boolean IsEmpty { get { return this._blocks.Count > 0; } }

		/// <summary>Get signature block by block ID or null if block is not found</summary>
		/// <param name="id">Block identifier for required schema</param>
		/// <returns>Strongly typed APK signature block</returns>
		public ApkSignatureBlock this[ApkSignatureBlock.BlockId id]
		{
			get
			{
				ApkSignatureBlock result = null;
				return _blocks.TryGetValue(id, out result) ? result : (ApkSignatureBlock)null;
			}
		}

		/// <summary>Try to get signature block by strongly typed block wrapper</summary>
		/// <typeparam name="T">Strongly typed block wrapper type</typeparam>
		/// <returns>Found strongly typed signature block</returns>
		/// <exception cref="NotSupportedException">Only V2 signature block supported</exception>
		public T GetBlockByType<T>() where T : ApkSignatureBlock
		{
			if(typeof(T) == typeof(ApkSignatureV2Block))
			{
				ApkSignatureBlock result;
				return _blocks.TryGetValue(ApkSignatureBlock.BlockId.APK_SIGNATURE_SCHEME_V2_BLOCK_ID, out result)
					? (T)result : (T)null;
			} else
				throw new NotImplementedException();
		}

		/// <summary>Get all found signature blocks</summary>
		/// <returns>List of signature blocks</returns>
		public IEnumerator<ApkSignatureBlock> GetEnumerator()
		{
			return _blocks.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal ApkSignature(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException(nameof(stream));
			if(stream.CanSeek == false || stream.CanRead == false)
				throw new ArgumentException("stream is readonly", nameof(stream));

			using(BinaryReader br = new BinaryReader(stream))
			{
				//Trying to find EoCD. It's in always in the end of a zip archive
				br.BaseStream.Position = br.BaseStream.Length - Marshal.SizeOf(typeof(EndOfCentralDirectoryFileHeader));
				while(true)
				{
					if(br.ReadInt32() == EndOfCentralDirectoryFileHeader.SignatureValue)
					{
						br.BaseStream.Seek(-sizeof(Int32), SeekOrigin.Current);
						break;
					} else//We need this block of EoCD ends with file comments. Normally, android package does't contains comments, but wee need to check
						br.BaseStream.Seek(-(sizeof(Int32) + 1), SeekOrigin.Current);//BUG: If we will not find EoCD signture, then we can find damaged file
				}

				EndOfCentralDirectoryFileHeader eocd = Utils.PtrToStructure<EndOfCentralDirectoryFileHeader>(br);
				if(eocd.IsValid == false)
					return;

				br.BaseStream.Position = eocd.offsetToCentralDirectory;

				br.BaseStream.Seek(-Marshal.SizeOf(typeof(ApkSignatureSchemeHeader)), SeekOrigin.Current);
				Int64 apkSignatureBlockEnd = br.BaseStream.Position;
				ApkSignatureSchemeHeader signatureHeader = Utils.PtrToStructure<ApkSignatureSchemeHeader>(br);
				if(signatureHeader.IsValid == false)
					return;

				Int32 apkSignatureSchemeStart = eocd.offsetToCentralDirectory - (Int32)signatureHeader.sizeOfScheme;

				br.BaseStream.Position = apkSignatureSchemeStart;
				while(br.BaseStream.Position < apkSignatureBlockEnd)
				{
					UInt64 sizeOfBlock2 = br.ReadUInt64();
					ApkSignatureBlock.BlockId id = (ApkSignatureBlock.BlockId)br.ReadUInt32();
					Byte[] blockData = br.ReadBytes((Int32)(sizeOfBlock2 - sizeof(UInt32)));

					ApkSignatureBlock signature;
					switch(id)
					{
					case ApkSignatureBlock.BlockId.APK_SIGNATURE_SCHEME_V2_BLOCK_ID:
						signature = new ApkSignatureV2Block(blockData);
						break;
					default:
						signature = new ApkSignatureBlock(id, blockData);
						break;
					}
					_blocks.Add(id, signature);
				}
			}
		}
	}
}