using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

using AlphaOmega.Debug.Signature;

using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

using static AlphaOmega.Debug.ApkSignature.Api;

namespace AlphaOmega.Debug
{
	public class ApkSignature
	{
		internal sealed class Api
		{
			[StructLayout(LayoutKind.Sequential, Pack = 2)]
			public struct EndOfCentralDirectoryFileHeader
			{
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

				public Boolean IsValid { get { return signature == SignatureValue; } }
			}

			[StructLayout(LayoutKind.Sequential, Pack = 2)]
			public struct ApkSignatureSchemeHeader
			{
				public UInt64 sizeOfScheme;
				[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
				public Byte[] magic;
				public String MagicStr { get { return System.Text.Encoding.ASCII.GetString(magic); } }
				public Boolean IsValud { get { return MagicStr == "APK Sig Block 42"; } }
			}
		}
		private List<ApkSignatureInfo> _blocks;

		public IEnumerable<ApkSignatureInfo> Blocks { get { return _blocks.AsReadOnly(); } }

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
				br.BaseStream.Position = eocd.offsetToCentralDirectory;

				br.BaseStream.Seek(-(Marshal.SizeOf(typeof(ApkSignatureSchemeHeader))), SeekOrigin.Current);
				Int64 apkSignatureBlockEnd = br.BaseStream.Position;
				ApkSignatureSchemeHeader apkHeader = Utils.PtrToStructure<ApkSignatureSchemeHeader>(br);
				Int32 apkSignatureSchemeStart = eocd.offsetToCentralDirectory - (Int32)apkHeader.sizeOfScheme;

				br.BaseStream.Position = apkSignatureSchemeStart;
				_blocks = new List<ApkSignatureInfo>();
				while(br.BaseStream.Position < apkSignatureBlockEnd)
				{
					UInt64 sizeOfBlock2 = br.ReadUInt64();
					UInt32 id = br.ReadUInt32();
					Byte[] blockData = br.ReadBytes((Int32)(sizeOfBlock2 - sizeof(UInt32)));
                    ApkSignatureInfo signature = new ApkSignatureInfo(id, blockData);

					_blocks.Add(signature);
				}
			}
		}
	}
}