using System;
using System.Diagnostics;

namespace AlphaOmega.Debug.Data.Tables
{
	/// <summary>
	/// Field identifiers list.
	/// These are identifiers for all fields referred to by this file, whether defined in the file or not.
	/// </summary>
	/// <remarks>
	/// This list must be sorted, where the defining type (by type_id index) is the major order, field name (by string_id index) is the intermediate order, and type (by type_id index) is the minor order.
	/// The list must not contain any duplicate entries.
	/// </remarks>
	[DebuggerDisplay("{type_idx.descriptor_idx.data} {name_idx.data} (Owner: {class_idx.descriptor_idx.data})")]
	public class field_id_row : BaseRow
	{
		private UInt16 class_idxI { get { return base.GetValue<UInt16>(0); } }

		private UInt16 type_idxI { get { return base.GetValue<UInt16>(1); } }

		private UInt32 name_idxI { get { return base.GetValue<UInt32>(2); } }

		/// <summary>
		/// Index into the type_ids list for the definer of this field.
		/// This must be a class type, and not an array or primitive type.
		/// </summary>
		public type_id_row class_idx { get { return base.File.TYPE_ID_ITEM[this.class_idxI]; } }

		/// <summary>Index into the type_ids list for the type of this field.</summary>
		public type_id_row type_idx { get { return base.File.TYPE_ID_ITEM[this.type_idxI]; } }

		/// <summary>
		/// Index into the string_ids list for the name of this field.
		/// The string must conform to the syntax for MemberName, defined above.
		/// </summary>
		public string_data_row name_idx { get { return base.File.STRING_DATA_ITEM[this.name_idxI]; } }
	}
}