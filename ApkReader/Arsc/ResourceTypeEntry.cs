using System;

namespace AlphaOmega.Debug.Arsc
{
	/// <summary>Resource type entry</summary>
	public class ResourceTypeEntry
	{
		/// <summary>Resource id</summary>
		public Int32 ResourceId { get; }

		/// <summary>Resource entry</summary>
		public ArscApi.ResTable_entry Entry { get; }

		internal ResourceTypeEntry(Int32 resourceId, ArscApi.ResTable_entry entry)
		{
			this.ResourceId = resourceId;
			this.Entry = entry;
		}
	}
}