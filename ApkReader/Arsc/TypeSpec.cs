using System;
using System.IO;

namespace AlphaOmega.Debug.Arsc
{
	/// <summary>Structure defining the flags for a block of common resources</summary>
	public class TypeSpec
	{
		/// <summary>TypeSpec header</summary>
		public ArscApi.ResTable_typeSpec Header { get; }

		/// <summary>flags</summary>
		public Int32[] Flags { get; }

		internal TypeSpec(Byte[] buffer)
		{
			using(MemoryStream stream = new MemoryStream(buffer))
			using(BinaryReader reader = new BinaryReader(stream))
			{
				this.Header = Utils.PtrToStructure<ArscApi.ResTable_typeSpec>(reader);

				Int32[] flags = new Int32[this.Header.entryCount];
				for(Int32 i = 0; i < this.Header.entryCount; ++i)
					flags[i] = reader.ReadInt32();

				this.Flags = flags;
			}
		}
	}
}