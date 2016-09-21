using System;

namespace AlphaOmega.Debug.Data
{
	/// <summary>Type of the column from</summary>
	public enum ColumnType
	{
		/// <summary>Byte column</summary>
		Byte,
		/// <summary>Unsigned short</summary>
		UInt16,
		/// <summary>Unsigned integer</summary>
		UInt32,
		/// <summary>Signed Little-Endian Base 128</summary>
		SLeb128,
		/// <summary>Unsigned Little-Endian Base 128</summary>
		ULeb128,
		/// <summary>String column type (ULeb128+Data)</summary>
		String,

		/// <summary>Short integer array. Must begin with itemsCount integer</summary>
		UInt16Array,
		/// <summary>Integer array. Must begin with itemsCount integer</summary>
		UInt32Array,

		/// <summary>Generic payload</summary>
		Payload,
	}
}