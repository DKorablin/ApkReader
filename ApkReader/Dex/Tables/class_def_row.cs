using System;
using System.Diagnostics;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>Class definitions list.</summary>
	/// <remarks>
	/// The classes must be ordered such that a given class's superclass and implemented interfaces appear in the list earlier than the referring class.
	/// Furthermore, it is invalid for a definition for the same-named class to appear more than once in the list.
	/// </remarks>
	[DebuggerDisplay("{" + nameof(access_flags) + "} {class_idx.descriptor_idx.data} (Source: {" + nameof(source_file_idx) + "})")]
	public class class_def_row : BaseRow
	{
		private UInt32 class_idxI => base.GetValue<UInt32>(0);

		/// <summary>
		/// Index into the type_ids list for this class.
		/// This must be a class type, and not an array or primitive type.
		/// </summary>
		public type_id_row class_idx => base.File.TYPE_ID_ITEM[this.class_idxI];

		/// <summary>Access flags for the class (public, final, etc.).</summary>
		public DexApi.ACC access_flags => base.GetValue<DexApi.ACC>(1);

		private UInt32 superclass_idxI => base.GetValue<UInt32>(2);

		/// <summary>
		/// Index into the type_ids list for the superclass, or the constant value <see cref="DexApi.NO_INDEX"/> if this class has no superclass (i.e., it is a root class such as Object).
		/// If present, this must be a class type, and not an array or primitive type.
		/// </summary>
		public type_id_row superclass_idx
			=> this.superclass_idxI == DexApi.NO_INDEX
				? null
				: base.File.TYPE_ID_ITEM[this.superclass_idxI];

		private UInt32 interfaces_offI => base.GetValue<UInt32>(3);

		/// <summary>Offset from the start of the file to the list of interfaces, or 0 if there are none.</summary>
		/// <remarks>
		/// This offset should be in the data section, and the data there should be in the format specified by "type_list" below.
		/// Each of the elements of the list must be a class type (not an array or primitive type), and there must not be any duplicates.
		/// </remarks>
		public type_list_row interfaces_off
			=>  this.interfaces_offI == 0
				? null
				: base.File.TYPE_LIST.GetRowByOffset(this.interfaces_offI);

		private UInt32 source_file_idxI => base.GetValue<UInt32>(4);

		/// <summary>
		/// Index into the string_ids list for the name of the file containing the original source for (at least most of) this class, or the special value <see cref="DexApi.NO_INDEX"/> to represent a lack of this information.
		/// The debug_info_item of any given method may override this source file, but the expectation is that most classes will only come from one source file.
		/// </summary>
		public string_data_row source_file_idx
			=> this.source_file_idxI == DexApi.NO_INDEX
				? null
				: base.File.STRING_DATA_ITEM[this.source_file_idxI];

		/// <summary>
		/// Offset from the start of the file to the annotations structure for this class, or 0 if there are no annotations on this class.
		/// This offset, if non-zero, should be in the data section, and the data there should be in the format specified by "annotations_directory_item" below, with all items referring to this class as the definer.
		/// </summary>
		public UInt32 annotations_off => base.GetValue<UInt32>(5);

		private UInt32 class_data_offI => base.GetValue<UInt32>(6);

		/// <summary>
		/// Offset from the start of the file to the associated class data for this item, or 0 if there is no class data for this class.
		/// (This may be the case, for example, if this class is a marker interface.)
		/// The offset, if non-zero, should be in the data section, and the data there should be in the format specified by "class_data_item" below, with all items referring to this class as the definer.
		/// </summary>
		public class_data_row class_data_off
			=> this.class_data_offI == 0
				? null
				: base.File.CLASS_DATA_ITEM.GetRowByOffset(this.class_data_offI);

		/// <summary>
		/// Offset from the start of the file to the list of initial values for static fields, or 0 if there are none (and all static fields are to be initialized with 0 or null).
		/// This offset should be in the data section, and the data there should be in the format specified by "encoded_array_item" below.
		/// The size of the array must be no larger than the number of static fields declared by this class, and the elements correspond to the static fields in the same order as declared in the corresponding field_list.
		/// The type of each array element must match the declared type of its corresponding field.
		/// If there are fewer elements in the array than there are static fields, then the leftover fields are initialized with a type-appropriate 0 or null.
		/// </summary>
		public UInt32 static_values_off => base.GetValue<UInt32>(7);
	}
}