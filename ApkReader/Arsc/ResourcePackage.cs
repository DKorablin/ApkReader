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

		/// <summary>Package header</summary>
		public ArscApi.PackageHeader Header { get; }

		/// <summary>Type string pool</summary>
		public StringPool TypeStringPool { get; }

		/// <summary>Key string pool</summary>
		public StringPool KeyStringPool { get; }

		/// <summary>Type tables</summary>
		public List<ResourceType> TypeTable { get; } = new List<ResourceType>();

		/// <summary>Type spec tables</summary>
		public List<TypeSpec> TypeSpecTable { get; } = new List<TypeSpec>();

		internal ResourcePackage(Byte[] buffer)
		{
			using(MemoryStream stream = new MemoryStream(buffer))
			using(BinaryReader reader = new BinaryReader(stream))
			{
				this.Header = Utils.PtrToStructure<ArscApi.PackageHeader>(reader);

				if(!this.Header.IsValid)
					throw new InvalidOperationException("TypeStrings must immediately follow the package structure header.");

				this.TypeStringPool = new StringPool(this.Header.typeStrings_addr, reader);

				this.KeyStringPool = new StringPool(this.Header.keyStrings_addr, reader);

				reader.BaseStream.Seek(this.Header.keyStrings_addr + this.KeyStringPool.Header.header.size, SeekOrigin.Begin);
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
						this.TypeSpecTable.Add(flags);
						break;
					case RES_TABLE_TYPE_TYPE:// Process the package
						ResourceType resourceType = new ResourceType(this.Header.id, tableBuffer);
						this.TypeTable.Add(resourceType);
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