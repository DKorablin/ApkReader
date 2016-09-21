using System;
using System.Diagnostics;

namespace AlphaOmega.Debug.Data.Tables
{
	/// <summary>
	/// Type identifiers list.
	/// These are identifiers for all types (classes, arrays, or primitive types) referred to by this file, whether defined in the file or not.
	/// </summary>
	/// <remarks>This list must be sorted by string_id index, and it must not contain any duplicate entries.</remarks>
	[DebuggerDisplay("descriptor_idx={descriptor_idx.data}")]
	public class type_id_row : BaseRow
	{
		private UInt32 descriptor_idxI { get { return base.GetValue<UInt32>(0); } }

		/// <summary>
		/// Index into the string_ids list for the descriptor string of this type.
		/// The string must conform to the syntax for TypeDescriptor, defined above.
		/// </summary>
		public string_data_row descriptor_idx { get { return base.File.STRING_DATA_ITEM[this.descriptor_idxI]; } }

		/// <summary>A TypeDescriptor is the representation of any type, including primitives, classes, arrays, and void.</summary>
		public String TypeDescriptor
		{
			get
			{
				String descriptor = this.descriptor_idx.data;
				if(descriptor[0] == 'L')
				{
					return descriptor.Remove(0, 1).Replace('/', '.');
				}
				return descriptor;
			}
		}
	}
}