using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AlphaOmega.Debug.Arsc
{
	/// <summary>Resource type</summary>
	public class ResourceType
	{
		private readonly Int32 _packageId;
		private ArscApi.ResTable_Type _header;
		private Int32[] _entryIndices;
		private Dictionary<Int32,ResourceTypeEntrySimple> _simple;
		private Dictionary<Int32,ResourceTypeEntryComplex> _complex;

		/// <summary>Package id</summary>
		public Int32 PackageId { get { return this._packageId; } }

		/// <summary>Resource type header</summary>
		public ArscApi.ResTable_Type Header { get{ return this._header; } }

		/// <summary>Entry indexes</summary>
		public Int32[] EntryIndices { get { return this._entryIndices; } }

		/// <summary>Array of simple resources</summary>
		public Dictionary<Int32, ResourceTypeEntrySimple> Simple { get { return this._simple; } }

		/// <summary>Array of comples resources</summary>
		public Dictionary<Int32, ResourceTypeEntryComplex> Complex { get { return this._complex; } }
		
		internal ResourceType(Int32 packageId, Byte[] buffer)
		{
			this._packageId = packageId;
			this.Initialize(packageId, buffer);
		}

		private void Initialize(Int32 packageId, Byte[] buffer)
		{
			using(MemoryStream stream = new MemoryStream(buffer))
			using(BinaryReader reader = new BinaryReader(stream))
			{
				this._header = Utils.PtrToStructure<ArscApi.ResTable_Type>(reader);
				if(!this._header.IsValid)
					throw new InvalidOperationException("HeaderSize, entryCount and entriesStart are not valid.");

				// Skip config data
				reader.BaseStream.Seek(this._header.header.headerSize, SeekOrigin.Begin);

				this._entryIndices = new Int32[this._header.entryCount];
				for(Int32 i = 0; i < this._header.entryCount; ++i)
					this._entryIndices[i] = reader.ReadInt32();

				this._simple = new Dictionary<Int32, ResourceTypeEntrySimple>();
				this._complex = new Dictionary<Int32, ResourceTypeEntryComplex>();

				for(Int32 i = 0; i < this._header.entryCount; ++i)
				{
					if(this._entryIndices[i] == -1)
						continue;

					Int32 resource_id = (packageId << 24) | (this._header.id << 16) | i;
					ArscApi.ResTable_entry entry = Utils.PtrToStructure<ArscApi.ResTable_entry>(reader);

					if(entry.IsComplex)
					{
						ArscApi.ResTable_map_entry mapEntry = Utils.PtrToStructure<ArscApi.ResTable_map_entry>(reader);

						ArscApi.ResTable_map[] map = new ArscApi.ResTable_map[mapEntry.count];
						for(Int32 j = 0; j < mapEntry.count; ++j)
							map[j] = Utils.PtrToStructure<ArscApi.ResTable_map>(reader);

						this._complex.Add(resource_id, new ResourceTypeEntryComplex(resource_id, entry, mapEntry, map));
					} else
					{
						ArscApi.Res_value value = Utils.PtrToStructure<ArscApi.Res_value>(reader);
						this._simple.Add(resource_id, new ResourceTypeEntrySimple(resource_id, entry, value));
					}
				}
			}
		}
	}
}