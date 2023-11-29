using System;

namespace AlphaOmega.Debug.Arsc
{
	/// <summary>Simple resource type entry</summary>
	public class ResourceTypeEntrySimple : ResourceTypeEntry
	{
		/// <summary>Raw resource value</summary>
		public ArscApi.Res_value Value { get; }

		internal ResourceTypeEntrySimple(Int32 resourceId, ArscApi.ResTable_entry entry, ArscApi.Res_value value)
			: base(resourceId, entry)
			=> this.Value = value;
	}
}