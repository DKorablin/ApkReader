using System;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>code payload</summary>
	public class code_row : BaseRow
	{
		internal enum columns
		{
			register_size = 0,
			ins_size = 1,
			outs_size = 2,
			tries_size = 3,
			debug_info_off = 4,
			insns = 5,
			tries = 6,
			handlers = 7,
		}

		/// <summary>The number of registers used by this code</summary>
		public UInt16 registers_size { get { return base.GetValue<UInt16>((UInt16)columns.register_size); } }

		/// <summary>The number of words of incoming arguments to the method that this code is for</summary>
		public UInt16 ins_size { get { return base.GetValue<UInt16>((UInt16)columns.ins_size); } }

		/// <summary>The number of words of outgoing argument space required by this code for method invocation</summary>
		public UInt16 outs_size { get { return base.GetValue<UInt16>((UInt16)columns.outs_size); } }

		/// <summary>The number of try_items for this instance.</summary>
		/// <remarks>If non-zero, then these appear as the tries array just after the insns in this instance.</remarks>
		public UInt16 tries_size { get { return base.GetValue<UInt16>((UInt16)columns.tries_size); } }

		/// <summary>Offset from the start of the file to the debug info (line numbers + local variable info) sequence for this code, or 0 if there simply is no information.</summary>
		/// <remarks>
		/// The offset, if non-zero, should be to a location in the data section.
		/// The format of the data is specified by "debug_info_item" below.
		/// </remarks>
		public UInt32 debug_info_off { get { return base.GetValue<UInt32>((UInt16)columns.debug_info_off); } }

		/// <summary>
		/// Actual array of bytecode.
		/// The format of code in an insns array is specified by the companion document Dalvik bytecode.
		/// </summary>
		/// <remarks>
		/// Note that though this is defined as an array of ushort, there are some internal structures that prefer four-byte alignment.
		/// Also, if this happens to be in an endian-swapped file, then the swapping is only done on individual ushorts and not on the larger internal structures.
		/// </remarks>
		public UInt16[] insns { get { return base.GetValue<UInt16[]>((UInt16)columns.insns); } }

		private UInt32[] triesI { get { return base.GetValue<UInt32[]>((UInt16)columns.tries); } }

		/// <summary>Array indicating where in the code exceptions are caught and how to handle them.</summary>
		/// <remarks>
		/// Elements of the array must be non-overlapping in range and in order from low to high address.
		/// This element is only present if tries_size is non-zero.
		/// </remarks>
		public try_item_row[] tries
		{
			get
			{
				var table = base.File.try_item;
				return this.triesI.Length == 0
					? null
					: Array.ConvertAll(this.triesI, delegate(UInt32 rowIndex) { return table[rowIndex]; });
			}
		}

		private UInt32[] handlersI { get { return base.GetValue<UInt32[]>((UInt16)columns.handlers); } }

		/// <summary>
		/// Bytes representing a list of lists of catch types and associated handler addresses.
		/// Each try_item has a byte-wise offset into this structure.
		/// </summary>
		/// <remarks>This element is only present if tries_size is non-zero.</remarks>
		public encoded_catch_handler_row[] handlers
		{
			get
			{
				var table = base.File.encoded_catch_handler_list;

				return this.handlersI == null
					? null
					: Array.ConvertAll(this.handlersI, delegate(UInt32 rowIndex) { return table[rowIndex]; });
			}
		}
	}
}