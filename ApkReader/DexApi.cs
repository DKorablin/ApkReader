using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;

namespace AlphaOmega.Debug
{
	/// <summary>Dalvik Executable format specifications</summary>
	public struct DexApi
	{
		/// <summary>The constant NO_INDEX is used to indicate that an index value is absent (-1 if treated as a signed int).</summary>
		/// <remarks>This value isn't defined to be 0, because that is in fact typically a valid index.</remarks>
		public const UInt32 NO_INDEX = 0xffffffff;

		/// <summary>The constant ENDIAN_CONSTANT is used to indicate the endianness of the file in whitch it is found.</summary>
		/// <remarks>
		/// Although the standart .dex format is little-endian, implementations may chooseto perform byte-swapping.
		/// Should an implementation come across a header whose endian_tag is REVERSE_ENDIAN_CONSTANT instead of ENDIAN_CONSTANT, it would know that the file has been byte-swapping from the expected form.
		/// </remarks>
		public enum ENDIAN : uint
		{
			/// <summary>Little-endian</summary>
			ENDIAN = 0x12345678,
			/// <summary>Big-endian</summary>
			REVERSE_ENDIAN = 0x78563412,
		}

		/// <summary>Dalvik header</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct header_item
		{
			/// <summary>magic value</summary>
			/// <remarks>
			/// The constant array/string DEX_FILE_MAGIC is the list of bytes that must appear at the beginning of a .dex file in order for it to be recognized as such.
			/// The value intentionally contains a newline ("\n" or 0x0a) and a null byte ("\0" or 0x00) in order to help in the detection of certain forms of corruption.
			/// The value also encodes a format version number as three decimal digits, which is expected to increase monotonically over time as the format evolves.
			/// </remarks>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public Byte[] magic;

			/// <summary>Used to detect corruption</summary>
			/// <remarks>adler32 checksum of the reset of the file (everything but magic and this field)</remarks>
			public UInt32 checksum;

			/// <summary>Used to uniquely identify files</summary>
			/// <remarks>SHA-1 signature (hash) of the rest of the file (everything but magic, checksum, and this field)</remarks>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
			public Byte[] signature;

			/// <summary>Size of the entire file (including the header), in bytes.</summary>
			public UInt32 file_size;

			/// <summary>A limited amount of backwards/forwards compability without invalidating the format.</summary>
			public UInt32 header_size;

			/// <summary>Endianness tag.</summary>
			public DexApi.ENDIAN endian_tag;

			/// <summary>Size of the link section, or 0 if this file isn't statically linked</summary>
			public UInt32 link_size;

			/// <summary>
			/// Offset from the start of the file to the link section, or 0 if link_size == 0.
			/// The offset, if non-zero, should be to an offset into the link_data section.
			/// The format of the data pointed at is left unspecified by this document; this header field (and the previous) are left as hooks for use by runtime implementations.
			/// </summary>
			public UInt32 link_off;

			/// <summary>
			/// Offset from the start of the file to the map item.
			/// The offset, which must be non-zero, should be to an offset into the data section, and the data should be in the format specified by "map_list" below.
			/// </summary>
			public UInt32 map_off;

			/// <summary>Count of strings in the string identifiers list</summary>
			public UInt32 string_ids_size;

			/// <summary>
			/// Offset from the start of the file to the string identifiers list, or 0 if string_ids_size == 0 (admittedly a strange edge case).
			/// The offset, if non-zero, should be to the start of the string_ids section.
			/// </summary>
			public UInt32 string_ids_off;

			/// <summary>Count of elements in the type identifiers list, at most 65535</summary>
			public UInt32 type_ids_size;

			/// <summary>
			/// Offset from the start of the file to the type identifiers list, or 0 if type_ids_size == 0 (admittedly a strange edge case).
			/// The offset, if non-zero, should be to the start of the type_ids section.
			/// </summary>
			public UInt32 type_ids_off;

			/// <summary>Count of elements in the prototype identifiers list, at most 65535</summary>
			public UInt32 proto_ids_size;

			/// <summary>
			/// Offset from the start of the file to the prototype identifiers list, or 0 if proto_ids_size == 0 (admittedly a strange edge case).
			/// The offset, if non-zero, should be to the start of the proto_ids section.
			/// </summary>
			public UInt32 proto_ids_off;

			/// <summary>Count of elements in the field identifiers list</summary>
			public UInt32 field_ids_size;

			/// <summary>
			/// Offset from the start of the file to the field identifiers list, or 0 if field_ids_size == 0.
			/// The offset, if non-zero, should be to the start of the field_ids section.
			/// </summary>
			public UInt32 field_ids_off;

			/// <summary>Count of elements in the method identifiers list</summary>
			public UInt32 method_ids_size;

			/// <summary>
			/// Offset from the start of the file to the method identifiers list, or 0 if method_ids_size == 0.
			/// The offset, if non-zero, should be to the start of the method_ids section.
			/// </summary>
			public UInt32 method_ids_off;

			/// <summary>Count of elements in the class definitions list</summary>
			public UInt32 class_defs_size;

			/// <summary>
			/// Offset from the start of the file to the class definitions list, or 0 if class_defs_size == 0 (admittedly a strange edge case).
			/// The offset, if non-zero, should be to the start of the class_defs section.
			/// </summary>
			public UInt32 class_defs_off;

			/// <summary>Size of the data section in bytes.</summary>
			/// <remarks>Must be an even multiple of sizeof(uint)</remarks>
			public UInt32 data_size;

			/// <summary>Offset from the start of the file to the start of the data section</summary>
			public UInt32 data_off;

			/// <summary>String representation for magic field</summary>
			public String MagicStr { get { return Encoding.ASCII.GetString(this.magic).Replace("\n", "\\n").Replace("\x0", "\\0"); } }

			/// <summary>Validating DEX header magic</summary>
			public Boolean IsValid
			{
				get
				{
					return this.magic[0] == 0x64 && this.magic[1] == 0x65 && this.magic[2] == 0x78 && this.magic[3] == 0x0a;
				}
			}
		}

		/// <summary>Type of the map items</summary>
		public enum TYPE : ushort
		{
			/// <summary>Dex header pointer</summary>
			HEADER_ITEM = 0x0000,
			/// <summary>string identifiers list</summary>
			STRING_ID_ITEM = 0x0001,
			/// <summary>Type identifiers list</summary>
			TYPE_ID_ITEM = 0x0002,
			/// <summary>Method prototype identifiers list</summary>
			PROTO_ID_ITEM = 0x0003,
			/// <summary>Field identifiers list</summary>
			FIELD_ID_ITEM = 0x0004,
			/// <summary>Method identifiers list</summary>
			METHOD_ID_ITEM = 0x0005,
			/// <summary>Class definitions list</summary>
			CLASS_DEF_ITEM = 0x0006,
			/// <summary>
			/// This is a list of the entire contents of a file, in order.
			/// It contains some redundancy with respect to the header_item but is intended to be an easy form to use to iterate over an entire file.
			/// A given type must appear at most once in a map, but there is no restriction on what order types may appear in, other than the restrictions implied by the rest of the format (e.g., a header section must appear first, followed by a string_ids section, etc.).
			/// Additionally, the map entries must be ordered by initial offset and must not overlap.
			/// </summary>
			MAP_LIST = 0x1000,
			/// <summary>Type identifiers list</summary>
			TYPE_LIST = 0x1001,
			/// <summary>banana banana banana</summary>
			ANNOTATION_SET_REF_LIST = 0x1002,
			/// <summary>banana banana banana</summary>
			ANNOTATION_SET_ITEM = 0x1003,
			/// <summary>Class structure list</summary>
			CLASS_DATA_ITEM = 0x2000,
			/// <summary>Source bytecode payload</summary>
			CODE_ITEM = 0x2001,
			/// <summary>String identifiers list</summary>
			STRING_DATA_ITEM = 0x2002,
			/// <summary>Not implemented item</summary>
			DEBUG_INFO_ITEM = 0x2003,
			/// <summary>Not implemented item</summary>
			ANNOTATION_ITEM = 0x2004,
			/// <summary>Not implemented item</summary>
			ENCODED_ARRAY_ITEM = 0x2005,
			/// <summary>banana banana banana</summary>
			ANNOTATIONS_DIRECTORY_ITEM = 0x2006,
		}

		/// <summary>
		/// This is a list of the entire contents of a file, in order.
		/// It contains some redundancy with respect to the header_item but is intended to be an easy form to use to iterate over an entire file.
		/// A given type must appear at most once in a map, but there is no restriction on what order types may appear in, other than the restrictions implied by the rest of the format (e.g., a header section must appear first, followed by a string_ids section, etc.).
		/// Additionally, the map entries must be ordered by initial offset and must not overlap.
		/// </summary>
		[DebuggerDisplay("{type} ({size})")]
		[StructLayout(LayoutKind.Sequential)]
		public struct map_item
		{
			/// <summary>Type of the items</summary>
			public TYPE type;
			/// <summary>unused</summary>
			public UInt16 unused;
			/// <summary>Count of the number of items to be found at the indicated offset</summary>
			public UInt32 size;
			/// <summary>Offset from the start of the file to the items in question</summary>
			public UInt32 offset;
		}

		/// <summary>Bitfields of these flags are used to indicate the accessibility and overall properties of classes and class members.</summary>
		[Flags]
		public enum ACC : uint
		{
			/// <summary>
			/// Class: public: visible everywhere
			/// Method: public: visible everywhere
			/// Field: public: visible everywhere
			/// </summary>
			PUBLIC=0x1,
			/// <summary>
			/// Class: private: only visible to defining class
			/// Method: private: only visible to defining class
			/// Field: private: only visible to defining class
			/// </summary>
			PRIVATE=0x2,
			/// <summary>
			/// Class: protected: visible to package and subclasses 
			/// Method: protected: visible to package and subclasses
			/// Field: protected: visible to package and subclasses
			/// </summary>
			PROTECTED=0x4,
			/// <summary>
			/// Class: static: is not constructed with an outer this reference
			/// Method: static: does not take a this argument
			/// Field: static: global to defining class
			/// </summary>
			STATIC=0x8,
			/// <summary>
			/// Class: final: not subclassable
			/// Method: final: not overridable
			/// Field: final: immutable after construction
			/// </summary>
			FINAL=0x10,
			/// <summary>
			/// Class: none
			/// Method: synchronized: associated lock automatically acquired around call to this method. (This is only valid to set when ACC_NATIVE is also set.)
			/// Field: none
			/// </summary>
			SYNCHRONIZED=0x20,
			/// <summary>
			/// Class: none
			/// Method: bridge method, added automatically by compiler as a type-safe bridge
			/// Field: volatile: special access rules to help with thread safety
			/// </summary>
			VOLATILE_BRIDGE=0x40,
			/// <summary>
			/// Class: none
			/// Method: last argument should be treated as a "rest" argument by compiler
			/// Field: transient: not to be saved by default serialization
			/// </summary>
			TRANSIENT_VARARGS=0x80,
			/// <summary>
			/// Class: none
			/// Method: native: implemented in native code
			/// Field: none
			/// </summary>
			NATIVE=0x100,
			/// <summary>
			/// Class: interface: multiply-implementable abstract class
			/// Method: none
			/// Field: none
			/// </summary>
			INTERFACE=0x200,
			/// <summary>
			/// Class: abstract: not directly instantiable
			/// Method: abstract: unimplemented by this class
			/// Field: none
			/// </summary>
			ABSTRACT=0x400,
			/// <summary>
			/// Class: none
			/// Method: strictfp: strict rules for floating-point arithmetic
			/// Field: none
			/// </summary>
			STRICT=0x800,
			/// <summary>
			/// Class: not directly defined in source code
			/// Method: not directly defined in source code
			/// Field: not directly defined in source code
			/// </summary>
			SYNTHETIC=0x1000,
			/// <summary>
			/// Class: declared as an annotation class
			/// Method: none
			/// Field: none
			/// </summary>
			ANNOTATION=0x2000,
			/// <summary>
			/// Class: declared as an enumerated type
			/// Method: none
			/// Field: declared as an enumerated value
			/// </summary>
			ENUM=0x4000,
			/// <summary>
			/// Class: none
			/// Method: Constructor method (class or instance initializer)
			/// Field: none
			/// </summary>
			CONSTRUCTOR=0x10000,
			/// <summary>
			/// Class: none
			/// Method: declared synchronized.
			/// [This has no effect on execution (other than in reflection of this flag, per se).]
			/// Field: none
			/// </summary>
			DECLARED_SYNCHRONIZED = 0x20000,
		}
	}
}