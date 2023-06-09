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
		private IImageLoader _loader;
		private readonly AxmlApi.AxmlHeader _header;
		private UInt32[] _stringsOffset;
		private String[] _strings;
		private XmlNode _rootNode;

		/// <summary>Source loader</summary>
		private IImageLoader Loader { get { return this._loader; } }

		/// <summary>AXML header</summary>
		public AxmlApi.AxmlHeader Header { get { return this._header; } }

		/// <summary>Decoded XML root node</summary>
		public XmlNode RootNode
		{
			get
			{
				return this._rootNode == null
					? this._rootNode = this.ReadXmlNode()
					: this._rootNode;
			}
		}

		/// <summary>String table</summary>
		public String[] Strings
		{
			get
			{
				if(this._strings == null)
					this.ReadStrings();
				return this._strings;
			}
		}

		private UInt32[] StringsOffset
		{
			get
			{
				if(this._stringsOffset == null)
					this.ReadStrings();
				return this._stringsOffset;
			}
		}

		private UInt32 DataOffset
		{
			get
			{
				UInt32 offset = (UInt32)((UInt32)Marshal.SizeOf(typeof(AxmlApi.AxmlHeader))
					+ (this.Header.stringCount * sizeof(UInt32))
					+ this.Strings[this._strings.Length - 1].Length
					+ this.StringsOffset[this.StringsOffset.Length - 1]
					+ (UInt32)sizeof(UInt16));
				return Utils.AlignToInt(offset);
			}
		}

		/// <summary>Create instance of AXML file unpacker</summary>
		/// <param name="loader">Source loader</param>
		public AxmlFile(IImageLoader loader)
		{
			this._loader = loader?? throw new ArgumentNullException(nameof(loader));
			this._loader.Endianness = EndianHelper.Endian.Little;

			this._header = this.Loader.PtrToStructure<AxmlApi.AxmlHeader>(0);
		}

		/// <summary>Read strings table</summary>
		private void ReadStrings()
		{
			if(!this.Header.IsValid)
				throw new InvalidOperationException("Invalid header");

			UInt32 offset = (UInt32)Marshal.SizeOf(typeof(AxmlApi.AxmlHeader));
			this._strings = new String[this.Header.stringCount];
			this._stringsOffset = new UInt32[this.Header.stringCount];
			for(Int32 loop = 0; loop < this._stringsOffset.Length; loop++)
			{
				this._stringsOffset[loop] = this.Loader.PtrToStructure<UInt32>(offset);
				offset += sizeof(UInt32);
			}

			for(Int32 loop = 0; loop < this._strings.Length; loop++)
			{
				UInt32 startOffset = offset + this._stringsOffset[loop];

				Int16 check = this.Loader.PtrToStructure<Int16>(startOffset);
				Int32 stringSize = check & 0xff;//String lengh + 1 byte(?)
				startOffset += sizeof(Int16);

				this._strings[loop] = this.Header.IsAsciiEncoding
					? this.Loader.PtrToStringAnsi(startOffset)
					: Encoding.Unicode.GetString(this.Loader.ReadBytes(startOffset, (UInt32)stringSize * 2));

				/*if(stringSize != this._strings[loop].Length)
					throw new InvalidOperationException($"String size misbehave: Expected: {stringSize} Collected: {this._strings[loop].Length}");*/
			}

			/*for(Int32 loop = 0; loop < this._strings.Length; loop++)
			{
				UInt32 startOffset = offset + this._stringsOffset[loop] + sizeof(UInt16);//String lengh + 1 byte(?)
				this._strings[loop] = this.Loader.PtrToStringAnsi(startOffset);
			}*/
		}

		/// <summary>Read encoded AXML chunks</summary>
		/// <returns>AXML chunk descriptions stream</returns>
		public IEnumerable<AxmlChunk> ReadXmlChunks()
		{
			if(!this.Header.IsValid)
				throw new InvalidOperationException("Invalid header");

			UInt32 offset = (UInt32)this.Header.xmlOffset;
			while(offset < this.Header.FileSize)
			{//HACK: Тут лежит неизвестные данные, которые надо пропустить. Но есть шанс попасть в данные и не найти XML...
				Int32 startTag = this.Loader.PtrToStructure<Int32>(offset);
				if(startTag == (Int32)AxmlApi.ChunkType.StartTag)
					break;
				else
					offset += sizeof(Int32);
			}

			while(offset < this.Header.FileSize)
			{
				AxmlApi.Chunk chunk = this.Loader.PtrToStructure<AxmlApi.Chunk>(offset);
				offset += (UInt32)Marshal.SizeOf(typeof(AxmlApi.Chunk));

				switch((AxmlApi.ChunkType)chunk.TagType)
				{
				case AxmlApi.ChunkType.StartDocument:
				case AxmlApi.ChunkType.EndDocument:
					AxmlApi.Document sDoc = this.Loader.PtrToStructure<AxmlApi.Document>(offset);
					offset += (UInt32)Marshal.SizeOf(typeof(AxmlApi.Document));
					yield return new AxmlChunk(chunk, sDoc);
					break;
				case AxmlApi.ChunkType.StartTag:
					AxmlApi.StartTag sTag = this.Loader.PtrToStructure<AxmlApi.StartTag>(offset);
					offset += (UInt32)Marshal.SizeOf(typeof(AxmlApi.StartTag));

					AxmlApi.Attribute[] attributes = new AxmlApi.Attribute[sTag.attributeCount];
					for(Int32 loop = 0; loop < attributes.Length; loop++)
					{
						attributes[loop] = this.Loader.PtrToStructure<AxmlApi.Attribute>(offset);
						offset += (UInt32)Marshal.SizeOf(typeof(AxmlApi.Attribute));
					}

					yield return new AxmlChunk(chunk, sTag, attributes);
					break;
				case AxmlApi.ChunkType.EndTag:
					AxmlApi.EndTag eTag = this.Loader.PtrToStructure<AxmlApi.EndTag>(offset);
					offset += (UInt32)Marshal.SizeOf(typeof(AxmlApi.EndTag));
					yield return new AxmlChunk(chunk, eTag);
					break;
				case AxmlApi.ChunkType.Text:
					AxmlApi.Text text = this.Loader.PtrToStructure<AxmlApi.Text>(offset);
					offset += (UInt32)Marshal.SizeOf(typeof(AxmlApi.Text));
					yield return new AxmlChunk(chunk, text);
					break;
				default:
					throw new NotSupportedException($"Chunk type {chunk.TagType} not supported");
				}
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
				case AxmlApi.ChunkType.StartTag:
					XmlNode parentNode = node;
					node = new XmlNode(parentNode, this.Strings[chunk.StartTag.Value.tagName]);
					foreach(var attribute in chunk.Attributes)
						switch(attribute.ValueType)
						{
						case AxmlApi.AttributeValue.String:
							node.AddAttribue(this.Strings[attribute.name], this.Strings[attribute.value]);
							break;
						default:
							node.AddAttribue(this.Strings[attribute.name], attribute.value.ToString());
							break;
						}
					if(parentNode != null)
						parentNode.AddChildNode(node);
					break;
				case AxmlApi.ChunkType.EndTag:
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