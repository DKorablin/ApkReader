using System;

namespace AlphaOmega.Debug.Arsc
{
	/// <summary>Application friendly resource</summary>
	public class ResourceRow
	{
		private readonly ArscApi.Res_value _descriptor;
		private readonly String _stringValue;

		/// <summary>Data type</summary>
		public ArscApi.DATA_TYPE DataType => this._descriptor.dataType;

		/// <summary>Raw resource value</summary>
		public Int32 Raw => this._descriptor.data;

		/// <summary>String representation</summary>
		public String Value
		{
			get
			{
				switch(this._descriptor.dataType)
				{
				case ArscApi.DATA_TYPE.STRING:
					return this._stringValue;
				default:
					return this._descriptor.DataToString();
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