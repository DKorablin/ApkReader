using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using AlphaOmega.Debug.Axml;

namespace AlphaOmega.Debug
{
	/// <summary>Android XML file decoder</summary>
	public class AxmlFile : IDisposable
	{
		/// <summary>Source loader</summary>
		private IImageLoader _loader;
		private String[] _strings;
		private XmlNode _rootNode;

		/// <summary>AXML header</summary>
		public AxmlApi.AxmlFileHeader Header { get; }

		/// <summary>Decoded XML root node</summary>
		public XmlNode RootNode { get => this._rootNode ?? (this._rootNode = this.ReadXmlNode()); }

		/// <summary>String table</summary>
		public String[] Strings { get => this._strings ?? (this._strings = this.ReadStrings()); }

		/// <summary>Create instance of AXML file unpacker</summary>
		/// <param name="loader">Source loader</param>
		public AxmlFile(IImageLoader loader)
		{
			this._loader = loader ?? throw new ArgumentNullException(nameof(loader));
			this._loader.Endianness = EndianHelper.Endian.Little;

			this.Header = this._loader.PtrToStructure<AxmlApi.AxmlFileHeader>(0);
		}

		/// <summary>Read strings table</summary>
		private String[] ReadStrings()
		{
			if(!this.Header.IsValid)
				throw new InvalidOperationException("Invalid header");

			UInt32 offset = (UInt32)Marshal.SizeOf(typeof(AxmlApi.AxmlFileHeader));
			var result = new String[this.Header.StringPool.stringCount];
			var stringsOffset = new UInt32[this.Header.StringPool.stringCount];

			// 1. Read the offsets
			for(Int32 loop = 0; loop < stringsOffset.Length; loop++)
			{
				stringsOffset[loop] = this._loader.PtrToStructure<UInt32>(offset);
				offset += sizeof(UInt32);
			}

			// 2. Read the actual strings
			for(Int32 loop = 0; loop < result.Length; loop++)
			{
				UInt32 startOffset = offset + stringsOffset[loop];

				if(this.Header.StringPool.IsUtf8)
				{ // UTF-8
					// UTF-8 strings have two lengths: UTF-16 length and UTF-8 byte length
					// We skip the first (char length) and use the second (byte length)
					this.ReadLen8(ref startOffset); // Skip char length
					Int32 byteLen = this.ReadLen8(ref startOffset);

					Byte[] data = this._loader.ReadBytes(startOffset, (UInt32)byteLen);
					result[loop] = Encoding.UTF8.GetString(data);
				} else
				{// UTF-16
					Int32 charLen = this.ReadLen16(ref startOffset);
					Byte[] data = this._loader.ReadBytes(startOffset, (UInt32)charLen * 2);
					result[loop] = Encoding.Unicode.GetString(data);
				}
			}

			return result;
		}

		private Int32 ReadLen8(ref UInt32 offset)
		{
			Byte len = this._loader.PtrToStructure<Byte>(offset++);
			if((len & 0x80) != 0)
			{
				// If high bit is set, use next byte as well
				len = (Byte)((len & 0x7F) << 8);
				len |= this._loader.PtrToStructure<Byte>(offset++);
			}
			return len;
		}

		private Int32 ReadLen16(ref UInt32 offset)
		{
			Int16 len = this._loader.PtrToStructure<Int16>(offset);
			offset += 2;
			if((len & 0x8000) != 0)
			{
				len = (Int16)((len & 0x7FFF) << 16);
				len |= this._loader.PtrToStructure<Int16>(offset);
				offset += 2;
			}
			return len;
		}

		/// <summary>Read encoded AXML chunks</summary>
		/// <returns>AXML chunk descriptions stream</returns>
		public IEnumerable<AxmlChunk> ReadXmlChunks()
		{
			if(!this.Header.IsValid)
				throw new InvalidOperationException("Invalid header");

			UInt32 offset = ((UInt32)Marshal.SizeOf(typeof(AxmlApi.AxmlFileHeader))
				- (UInt32)Marshal.SizeOf(typeof(AxmlApi.StringPoolHeader)))
				+ this.Header.StringPool.ChunkSize;

			while(offset < this.Header.FileSize)
			{
				AxmlApi.Chunk chunk = this._loader.PtrToStructure<AxmlApi.Chunk>(offset);
				UInt32 currentDataPointer = checked((UInt32)(offset + chunk.HeaderSize));

				switch(chunk.TagType)
				{
				case AxmlApi.ChunkType.RES_XML_RESOURCE_MAP_TYPE:
					Int32 idCount = (Int32)((chunk.ChunkSize - chunk.HeaderSize) / 4);
					UInt32[] resourceIds = new UInt32[idCount];
					UInt32 dataOffset = checked((UInt32)(chunk.ChunkSize + chunk.HeaderSize));
					for(Int32 i = 0; i < idCount; i++)
					{
						resourceIds[i] = this._loader.PtrToStructure<UInt32>(dataOffset);
						dataOffset += sizeof(UInt32);
					}
					break;
				case AxmlApi.ChunkType.RES_XML_START_NAMESPACE_TYPE:
				case AxmlApi.ChunkType.RES_XML_END_NAMESPACE_TYPE:
					AxmlApi.Document sDoc = this._loader.PtrToStructure<AxmlApi.Document>(currentDataPointer);
					yield return new AxmlChunk(chunk, sDoc);
					break;
				case AxmlApi.ChunkType.RES_XML_START_ELEMENT_TYPE:
					AxmlApi.StartTag sTag = this._loader.PtrToStructure<AxmlApi.StartTag>(currentDataPointer);

					UInt32 attrPointer = currentDataPointer + (UInt32)Marshal.SizeOf(typeof(AxmlApi.StartTag));
					AxmlApi.Attribute[] attributes = new AxmlApi.Attribute[sTag.attributeCount];
					for(Int32 loop = 0; loop < attributes.Length; loop++)
					{
						attributes[loop] = this._loader.PtrToStructure<AxmlApi.Attribute>(attrPointer);
						attrPointer += (UInt32)Marshal.SizeOf(typeof(AxmlApi.Attribute));
					}

					yield return new AxmlChunk(chunk, sTag, attributes);
					break;
				case AxmlApi.ChunkType.RES_XML_END_ELEMENT_TYPE:
					AxmlApi.EndTag eTag = this._loader.PtrToStructure<AxmlApi.EndTag>(currentDataPointer);
					yield return new AxmlChunk(chunk, eTag);
					break;
				case AxmlApi.ChunkType.RES_XML_CDATA_TYPE:
					AxmlApi.Text text = this._loader.PtrToStructure<AxmlApi.Text>(currentDataPointer);
					yield return new AxmlChunk(chunk, text);
					break;
				default:
					throw new NotImplementedException($"Chunk type {chunk.TagType} not implemented");
				}

				offset += chunk.ChunkSize;
			}
		}

		/// <summary>Convert AXML chunks to XML nodes</summary>
		/// <returns>XML node</returns>
		private XmlNode ReadXmlNode()
		{
			if(!this.Header.IsValid)
				throw new InvalidOperationException("Invalid header");

			XmlNode node = null;
			foreach(AxmlChunk chunk in this.ReadXmlChunks())
				switch(chunk.Chunk.TagType)
				{
				case AxmlApi.ChunkType.RES_XML_START_ELEMENT_TYPE:
					XmlNode parentNode = node;
					node = new XmlNode(parentNode, this.Strings[chunk.StartTag.Value.tagName]);
					foreach(var attribute in chunk.Attributes)
					{
						String value;
						switch(attribute.ValueType)
						{
						case AxmlApi.DataType.REFERENCE:
							value = $"@{attribute.Data:X8}";
							break;
						case AxmlApi.DataType.STRING:
							value = this.Strings[attribute.Data];
							break;
						case AxmlApi.DataType.INT_BOOLEAN:
							value = attribute.Data == 0 ? "false" : "true";
							break;
						case AxmlApi.DataType.INT_COLOR_ARGB8:
							value = $"#{attribute.Data:X8}";
							break;
						default:
							value = attribute.Data.ToString();
							break;
						}
						node.AddAttribute(this.Strings[attribute.NameIndex], value);
					}
					if(parentNode != null)
						parentNode.AddChildNode(node);
					break;
				case AxmlApi.ChunkType.RES_XML_END_ELEMENT_TYPE:
					if(node.ParentNode != null)
						node = node.ParentNode;
					else//TODO: Document declaration missing
						return node;
					break;
				}
			return node;
		}

		/// <summary>dispose data reader and all managed resources</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Dispose managed objects</summary>
		/// <param name="disposing">Dispose managed objects</param>
		protected virtual void Dispose(Boolean disposing)
		{
			if(disposing && this._loader != null)
			{
				this._loader.Dispose();
				this._loader = null;
			}
		}
	}
}