using System;

namespace AlphaOmega.Debug.Arsc
{
	/// <summary>Simple resource type entry</summary>
	public class ResourceTypeEntrySimple : ResourceTypeEntry
	{
		private readonly ArscApi.Res_value _value;

		/// <summary>Raw resource value</summary>
		public ArscApi.Res_value Value { get { return this._value; } }

		internal ResourceTypeEntrySimple(Int32 resourceId, ArscApi.ResTable_entry entry, ArscApi.Res_value value)
			: base(resourceId, entry)
		{
			this._value = value;
		}
	}
}