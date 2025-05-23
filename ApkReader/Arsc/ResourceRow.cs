using System;

namespace AlphaOmega.Debug.Arsc
{
	/// <summary>Application friendly resource</summary>
	public class ResourceRow
	{
		private readonly ArscApi.Res_value _descriptor;
		private readonly String _stringValue;

		/// <summary>Data type</summary>
		public ResourceValueDecoder.ValueType DataType => this._descriptor.DataType;

		/// <summary>Raw resource value</summary>
		public Int32 Raw => this._descriptor.data;

		/// <summary>String representation</summary>
		public String Value
		{
			get
			{
				switch(this._descriptor.DataType)
				{
				case ResourceValueDecoder.ValueType.STRING:
					return this._stringValue;
				default:
					return ResourceValueDecoder.DataToString(this._descriptor.data, this._descriptor.DataType);
				}
			}
		}

		internal ResourceRow(ArscApi.Res_value descriptor, String stringValue)
		{
			this._descriptor = descriptor;
			this._stringValue = stringValue;
		}
	}
}