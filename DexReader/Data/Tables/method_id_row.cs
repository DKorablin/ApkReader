using System;
using System.Diagnostics;

namespace AlphaOmega.Debug.Data.Tables
{
	/// <summary>
	/// Method identifiers list.
	/// These are identifiers for all methods referred to by this file, whether defined in the file or not.
	/// </summary>
	/// <remarks>
	/// This list must be sorted, where the defining type (by type_id index) is the major order, method name (by string_id index) is the intermediate order, and method prototype (by proto_id index) is the minor order.
	/// The list must not contain any duplicate entries.
	/// </remarks>
	[DebuggerDisplay("{proto_idx.shorty_idx.data} {name_idx.data} (Owner: {class_idx.descriptor_idx.data})")]
	public class method_id_row : BaseRow
	{
		private UInt16 class_idxI { get { return base.GetValue<UInt16>(0); } }

		private UInt16 proto_idxI { get { return base.GetValue<UInt16>(1); } }

		private UInt32 name_idxI { get { return base.GetValue<UInt32>(2); } }

		/// <summary>
		/// index into the type_ids list for the definer of this method.
		/// This must be a class or array type, and not a primitive type.
		/// </summary>
		public type_id_row class_idx { get { return base.File.TYPE_ID_ITEM[this.class_idxI]; } }

		/// <summary>Index into the proto_ids list for the prototype of this method</summary>
		public proto_id_row proto_idx { get { return base.File.PROTO_ID_ITEM[this.proto_idxI]; } }

		/// <summary>
		/// Index into the string_ids list for the name of this method.
		/// The string must conform to the syntax for MemberName, defined above.
		/// </summary>
		public string_data_row name_idx { get { return base.File.STRING_DATA_ITEM[this.name_idxI]; } }
	}
}