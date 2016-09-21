using System;

namespace AlphaOmega.Debug.Data.Tables
{
	/// <summary>Where in the code exceptions are caught and how to handle them.</summary>
	public class try_item_row : BaseRow
	{
		/// <summary>
		/// Start address of the block of code covered by this entry.
		/// The address is a count of 16-bit code units to the start of the first covered instruction.
		/// </summary>
		public UInt32 start_addr { get { return base.GetValue<UInt32>(0); } }

		/// <summary>
		/// Number of 16-bit code units covered by this entry.
		/// The last code unit covered (inclusive) is start_addr + insn_count - 1.
		/// </summary>
		public UInt16 insn_count { get { return base.GetValue<UInt16>(1); } }

		/// <summary>
		/// Offset in bytes from the start of the associated encoded_catch_hander_list to the encoded_catch_handler for this entry.
		/// This must be an offset to the start of an encoded_catch_handler.
		/// </summary>
		public UInt16 handler_off { get { return base.GetValue<UInt16>(2); } }
	}
}