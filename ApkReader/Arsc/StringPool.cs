using System;
using System.IO;
using System.Text;

namespace AlphaOmega.Debug.Arsc
{
	/// <summary>Structure that houses a group of strings</summary>
	public class StringPool
	{
		/// <summary>String pool header</summary>
		public ArscApi.ResStringPool_Header Header { get; }

		/// <summary>Strings array</summary>
		public String[] Strings { get; }

		internal StringPool(Byte[] buffer)
		{
			using(MemoryStream stream = new MemoryStream(buffer))
			using(BinaryReader reader = new BinaryReader(stream))
			{
				this.Header = Utils.PtrToStructure<ArscApi.ResStringPool_Header>(reader);

				Int32[] offsets = new Int32[this.Header.stringCount];
				for(Int32 i = 0; i < this.Header.stringCount; ++i)
					offsets[i] = reader.ReadInt32();

				String[] strings = new String[this.Header.stringCount];

				for(Int32 i = 0; i < this.Header.stringCount; i++)
				{
					Int32 pos = this.Header.stringsStart + offsets[i];
					reader.BaseStream.Seek(pos, SeekOrigin.Begin);
					if(this.Header.IsUTF8)
					{
						Int32 u16len = reader.ReadByte(); // u16len
						if((u16len & 0x80) != 0)// larger than 128
							u16len = ((u16len & 0x7F) << 8) + reader.ReadByte();

						Int32 u8len = reader.ReadByte(); // u8len
						if((u8len & 0x80) != 0)// larger than 128
							u8len = ((u8len & 0x7F) << 8) + reader.ReadByte();

						if(u8len > 0)
							strings[i] = Encoding.UTF8.GetString(reader.ReadBytes(u8len));
					} else // UTF_16
					{
						Int32 u16len = reader.ReadUInt16();
						if((u16len & 0x8000) != 0)// larger than 32768
							u16len = ((u16len & 0x7FFF) << 16) + reader.ReadUInt16();

						if(u16len > 0)
							strings[i] = Encoding.Unicode.GetString(reader.ReadBytes(u16len * 2));
					}
				}

				this.Strings = strings;
			}
		}

		internal StringPool(Int32 offset, BinaryReader reader)
		{
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			this.Header = Utils.PtrToStructure<ArscApi.ResStringPool_Header>(reader);

			Int32[] offsets = new Int32[this.Header.stringCount];
			for(Int32 i = 0; i < this.Header.stringCount; ++i)
				offsets[i] = reader.ReadInt32();

			String[] strings = new String[this.Header.stringCount];

			for(Int32 i = 0; i < this.Header.stringCount; i++)
			{
				Int32 pos = this.Header.stringsStart + offsets[i];
				reader.BaseStream.Seek(pos, SeekOrigin.Begin);
				if(this.Header.IsUTF8)
				{
					Int32 u16len = reader.ReadByte(); // u16len
					if((u16len & 0x80) != 0)// larger than 128
						u16len = ((u16len & 0x7F) << 8) + reader.ReadByte();

					Int32 u8len = reader.ReadByte(); // u8len
					if((u8len & 0x80) != 0)// larger than 128
						u8len = ((u8len & 0x7F) << 8) + reader.ReadByte();

					if(u8len > 0)
						strings[i] = Encoding.UTF8.GetString(reader.ReadBytes(u8len));
				} else // UTF_16
				{
					Int32 u16len = reader.ReadUInt16();
					if((u16len & 0x8000) != 0)// larger than 32768
						u16len = ((u16len & 0x7FFF) << 16) + reader.ReadUInt16();

					if(u16len > 0)
						strings[i] = Encoding.Unicode.GetString(reader.ReadBytes(u16len * 2));
				}
			}

			this.Strings = strings;
		}
	}
}