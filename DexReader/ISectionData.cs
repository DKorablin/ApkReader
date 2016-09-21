using System;

namespace AlphaOmega.Debug
{
	/// <summary>Represents data from specific section</summary>
	public interface ISectionData
	{
		/// <summary>Gets the data from specific image section</summary>
		/// <returns>byte array</returns>
		Byte[] GetData();
	}
}