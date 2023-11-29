using System;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>Catch handler lists</summary>
	public class encoded_catch_handler_row : BaseRow
	{
		internal enum columns
		{
			size = 0,
			handlers = 1,
			catch_all_addr = 2,
		}

		/// <summary>
		/// Number of catch types in this list.
		/// If non-positive, then this is the negative of the number of catch types, and the catches are followed by a catch-all handler.
		/// </summary>
		/// <example>
		/// A size of 0 means that there is a catch-all but no explicitly typed catches.
		/// A size of 2 means that there are two explicitly typed catches and no catch-all.
		/// And a size of -1 means that there is one typed catch along with a catch-all.
		/// </example>
		public Int32 size => base.GetValue<Int32>((UInt16)columns.size);

		private UInt32[] handlersI => base.GetValue<UInt32[]>((UInt16)columns.handlers);

		/// <summary>Stream of abs(size) encoded items, one for each caught type, in the order that the types should be tested.</summary>
		public encoded_type_addr_pair_row[] handlers
		{
			get
			{
				var table = base.File.encoded_type_addr_pair;
				return Array.ConvertAll(this.handlersI, delegate(UInt32 rowIndex) { return table[rowIndex]; });
			}
		}

		/// <summary>Bytecode address of the catch-all handler.</summary>
		/// <remarks>This element is only present if size is non-positive.</remarks>
		public UInt32? catch_all_addr => base.GetValue<UInt32?>((UInt16)columns.catch_all_addr);
	}
}