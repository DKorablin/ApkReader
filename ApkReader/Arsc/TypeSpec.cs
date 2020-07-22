using System;
using System.IO;

namespace AlphaOmega.Debug.Arsc
{
	/// <summary>Structure defining the flags for a block of common resources</summary>
	public class TypeSpec
	{
		private readonly ArscApi.ResTable_typeSpec _header;
		private readonly Int32[] _flags;

		/// <summary>TypeSpec header</summary>
		public ArscApi.ResTable_typeSpec Header { get { return this._header; } }

		/// <summary>flags</summary>
		public Int32[] Flags { get { return this._flags; } }

		internal TypeSpec(Byte[] buffer)
		{
			using(MemoryStream stream = new MemoryStream(buffer))
			using(BinaryReader reader = new BinaryReader(stream))
			{
				this._header = Utils.PtrToStructure<ArscApi.ResTable_typeSpec>(reader);

				Int32[] flags = new Int32[this._header.entryCount];
				for(Int32 i = 0; i < this._header.entryCount; ++i)
					flags[i] = reader.ReadInt32();

				this._flags = flags;
			}
		}
	}
}