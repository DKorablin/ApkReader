using System;

namespace AlphaOmega.Debug.Arsc
{
	/// <summary>Complex resource type entry</summary>
	public class ResourceTypeEntryComplex : ResourceTypeEntry
	{
		private readonly ArscApi.ResTable_map_entry _mapEntry;
		private readonly ArscApi.ResTable_map[] _map;

		internal ResourceTypeEntryComplex(Int32 resourceId, ArscApi.ResTable_entry entry, ArscApi.ResTable_map_entry mapEntry, ArscApi.ResTable_map[] map)
			: base(resourceId, entry)
		{
			this._mapEntry = mapEntry;
			this._map = map;
		}
	}
}