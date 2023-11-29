using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AlphaOmega.Debug.Arsc
{
	/// <summary>Resource type</summary>
	public class ResourceType
	{
		/// <summary>Package id</summary>
		public Int32 PackageId { get; }

		/// <summary>Resource type header</summary>
		public ArscApi.ResTable_Type Header { get; private set; }

		/// <summary>Entry indexes</summary>
		public Int32[] EntryIndices { get; private set; }

		/// <summary>Array of simple resources</summary>
		public Dictionary<Int32, ResourceTypeEntrySimple> Simple { get; } = new Dictionary<Int32, ResourceTypeEntrySimple>();

		/// <summary>Array of comples resources</summary>
		public Dictionary<Int32, ResourceTypeEntryComplex> Complex { get; } = new Dictionary<Int32, ResourceTypeEntryComplex>();

		internal ResourceType(Int32 packageId, Byte[] buffer)
		{
			this.PackageId = packageId;
			this.Initialize(packageId, buffer);
		}

		private void Initialize(Int32 packageId, Byte[] buffer)
		{
			using(MemoryStream stream = new MemoryStream(buffer))
			using(BinaryReader reader = new BinaryReader(stream))
			{
				this.Header = Utils.PtrToStructure<ArscApi.ResTable_Type>(reader);
				if(!this.Header.IsValid)
					throw new InvalidOperationException("HeaderSize, entryCount and entriesStart are not valid.");

				// Skip config data
				reader.BaseStream.Seek(this.Header.header.headerSize, SeekOrigin.Begin);

				this.EntryIndices = new Int32[this.Header.entryCount];
				for(Int32 i = 0; i < this.Header.entryCount; ++i)
					this.EntryIndices[i] = reader.ReadInt32();

				for(Int32 i = 0; i < this.Header.entryCount; ++i)
				{
					if(this.EntryIndices[i] == -1)
						continue;

					Int32 resource_id = (packageId << 24) | (this.Header.id << 16) | i;
					ArscApi.ResTable_entry entry = Utils.PtrToStructure<ArscApi.ResTable_entry>(reader);

					if(entry.IsComplex)
					{
						ArscApi.ResTable_map_entry mapEntry = Utils.PtrToStructure<ArscApi.ResTable_map_entry>(reader);

						ArscApi.ResTable_map[] map = new ArscApi.ResTable_map[mapEntry.count];
						for(Int32 j = 0; j < mapEntry.count; ++j)
							map[j] = Utils.PtrToStructure<ArscApi.ResTable_map>(reader);

						this.Complex.Add(resource_id, new ResourceTypeEntryComplex(resource_id, entry, mapEntry, map));
					} else
					{
						ArscApi.Res_value value = Utils.PtrToStructure<ArscApi.Res_value>(reader);
						this.Simple.Add(resource_id, new ResourceTypeEntrySimple(resource_id, entry, value));
					}
				}
			}
		}
	}
}