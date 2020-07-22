using System;

namespace AlphaOmega.Debug.Arsc
{
	/// <summary>Resource type entry</summary>
	public class ResourceTypeEntry
	{
		private readonly Int32 _resourceId;
		private readonly ArscApi.ResTable_entry _entry;
		
		/// <summary>Resource id</summary>
		public Int32 ResourceId { get { return this._resourceId; } }

		/// <summary>Resource entry</summary>
		public ArscApi.ResTable_entry Entry { get { return this._entry; } }

		internal ResourceTypeEntry(Int32 resourceId, ArscApi.ResTable_entry entry)
		{
			this._resourceId = resourceId;
			this._entry = entry;
		}
	}
}