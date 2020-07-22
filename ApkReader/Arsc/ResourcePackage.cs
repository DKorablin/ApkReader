using System;
using System.Collections.Generic;
using System.IO;

namespace AlphaOmega.Debug.Arsc
{
	/// <summary>Resource package</summary>
	public class ResourcePackage
	{
		private const Int16 RES_TABLE_TYPE_TYPE = 0x0201;
		private const Int16 RES_TABLE_TYPE_SPEC_TYPE = 0x0202;

		private readonly ArscApi.PackageHeader _header;
		private readonly StringPool _typeStringPool;
		private readonly StringPool _keyStringPool;
		private readonly List<TypeSpec> _typeSpecTable = new List<TypeSpec>();
		private List<ResourceType> _typeTable = new List<ResourceType>();

		/// <summary>Package header</summary>
		public ArscApi.PackageHeader Header { get { return this._header; } }

		/// <summary>Type string pool</summary>
		public StringPool TypeStringPool { get { return this._typeStringPool; } }

		/// <summary>Key string pool</summary>
		public StringPool KeyStringPool { get { return this._keyStringPool; } }

		/// <summary>Type tables</summary>
		public List<ResourceType> TypeTable { get { return this._typeTable; } }

		/// <summary>Type spec tables</summary>
		public List<TypeSpec> TypeSpecTable { get { return this._typeSpecTable; } }

		internal ResourcePackage(Byte[] buffer)
		{
			using(MemoryStream stream = new MemoryStream(buffer))
			using(BinaryReader reader = new BinaryReader(stream))
			{
				this._header = Utils.PtrToStructure<ArscApi.PackageHeader>(reader);

				if(!this._header.IsValid)
					throw new Exception("TypeStrings must immediately follow the package structure header.");

				this._typeStringPool = new StringPool(this._header.typeStrings_addr, reader);

				this._keyStringPool = new StringPool(this._header.keyStrings_addr, reader);

				reader.BaseStream.Seek(this._header.keyStrings_addr + this._keyStringPool.Header.header.size, SeekOrigin.Begin);
				while(true)
				{
					Int32 position = (Int32)reader.BaseStream.Position;
					ArscApi.ResChunk_Header chunk = Utils.PtrToStructure<ArscApi.ResChunk_Header>(reader);
					reader.BaseStream.Seek(position, SeekOrigin.Begin);

					Byte[] tableBuffer = reader.ReadBytes(chunk.size);
					switch(chunk.type)
					{
					case RES_TABLE_TYPE_SPEC_TYPE:// Process the string pool
						TypeSpec flags = new TypeSpec(tableBuffer);
						this._typeSpecTable.Add(flags);
						break;
					case RES_TABLE_TYPE_TYPE:// Process the package
						ResourceType resourceType = new ResourceType(this._header.id, tableBuffer);
						this._typeTable.Add(resourceType);
						break;
					}

					reader.BaseStream.Seek(position + chunk.size, SeekOrigin.Begin);
					if(reader.BaseStream.Position == reader.BaseStream.Length)
						break;
				}
			}
		}
	}
}