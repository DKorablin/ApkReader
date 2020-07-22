using System;
using System.Runtime.InteropServices;

namespace AlphaOmega.Debug
{
	/// <summary>Resource file format specifications</summary>
	public static class ArscApi
	{
		/// <summary>Resource file header</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct ResTable_Header
		{
			/// <summary>Chunk header</summary>
			public ResChunk_Header header;

			/// <summary>The number of ResTable_package structures</summary>
			public Int32 packageCount;

			/// <summary>Resource table header is valid</summary>
			public Boolean IsValid { get { return this.header.type == (Int32)ResourceTableType.ResTableType; } }
		}

		/// <summary>Resource file type</summary>
		public enum ResourceTableType : short
		{
			/// <summary>Resource table ID</summary>
			ResTableType = 0x0002,
		}

		/// <summary>Chunk header structure</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct ResChunk_Header
		{
			/// <summary>Type identifier of this chunk.</summary>
			/// <remarks>The meaning of this value depends on the containing class.</remarks>
			public Int16 type;
			/// <summary>Size of the chunk header (in bytes).</summary>
			/// <remarks>Adding this value to the address of the chunk allows you to find the associated data (if any).</remarks>
			public Int16 headerSize;
			/// <summary>Total size of this chunk (in bytes).</summary>
			/// <remarks>
			/// This is the chunkSize plus the size of any data associated with the chunk.
			/// Adding this value to the chunk allows you to completely skip its contents.
			/// If this value is the same as chunkSize, there is no data associated with the chunk.
			///</remarks>
			public Int32 size;
		}

		/// <summary>Package header</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct PackageHeader
		{
			/// <summary>Chunk header</summary>
			public ResChunk_Header header;

			/// <summary>Package id; usually 0x7F for application resources</summary>
			public Int32 id;

			/// <summary>Package name</summary>
			/// <example>com.google.android</example>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
			public Byte[] name;

			/// <summary>Address to typeStrings array</summary>
			public Int32 typeStrings_addr;

			/// <summary>banana banana banana</summary>
			public Int32 lastPlublic_type;

			/// <summary>Address to keyStrings array</summary>
			public Int32 keyStrings_addr;

			/// <summary>banana banana banana</summary>
			public Int32 lastPublic_key;

			/// <summary>Package name string</summary>
			/// <example>com.google.android</example>
			public String nameStr { get { return System.Text.Encoding.Unicode.GetString(this.name); } }

			/// <summary>Check header structure for validation</summary>
			/// <remarks>TypeStrings must immediately follow the package structure header.</remarks>
			public Boolean IsValid { get { return this.typeStrings_addr == this.header.headerSize; } }
		}

		/// <summary>Structure that houses a group of strings</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct ResStringPool_Header
		{
			/// <summary>chunk header</summary>
			public ResChunk_Header header;

			/// <summary>Number of strings in this pool (number of uint32_t indices that follow in the data).</summary>
			public Int32 stringCount;

			/// <summary>Number of style span arrays in the pool (number of uint32_t indices follow the string indices).</summary>
			public Int32 styleCount;

			/// <summary>If set, the string index is sorted by the string values (based on strcmp16()).</summary>
			public Int32 flags;

			/// <summary>Index from the header of the string data</summary>
			public Int32 stringsStart;

			/// <summary>Index from the header of the style data</summary>
			public Int32 stylesStart;

			/// <summary>String pool is encoded in UTF-8.</summary>
			public Boolean IsUTF8 { get { return (this.flags & 256) != 0; } }

			// <summary>If set, the string index is sorted by the string values (based on strcmp16()).</summary>
			//public Boolean IsSorted { get { return (this.flags << 0); } }
		}

		/// <summary>A collection of resource entries for a particular resource data type.</summary>
		/// <remarks>
		/// Followed by an array of uint32_t defining the resource values, corresponding
		/// to the array of type strings in the ResTable_Package::typeStrings string
		/// block. Each of these hold an index from entriesStart; a value of NO_ENTRY
		/// means that entry is not defined.
		/// 
		/// There may be multiple of these chunks for a particular resource type,
		/// supply different configuration variations for the resource values of
		/// that type.
		/// 
		/// It would be nice to have an additional ordered index of entries, so
		/// we can do a binary search if trying to find a resource by string name.
		/// </remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct ResTable_Type
		{
			/// <summary>chunk header</summary>
			public ResChunk_Header header;

			/// <summary>banana banana banana</summary>
			public Byte id;

			/// <summary>banana banana banana</summary>
			public Byte res0;

			/// <summary>banana banana banana</summary>
			public Int16 res1;

			/// <summary>banana banana banana</summary>
			public Int32 entryCount;

			/// <summary>banana banana banana</summary>
			public Int32 entriesStart;

			/// <summary>banana banana banana</summary>
			public Int32 configSize;

			/// <summary>Res table config</summary>
			public ResTable_Config config;

			/// <summary>Check for valid header</summary>
			public Boolean IsValid { get { return this.header.headerSize + this.entryCount * 4 == this.entriesStart; } }
		}

		/// <summary>Res table config</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct ResTable_Config
		{
			#region screenLayout bits for layout direction
			private const Byte ACONFIGURATION_LAYOUTDIR_ANY = 0x00;
			private const Byte ACONFIGURATION_LAYOUTDIR_LTR = 0x01;
			private const Byte ACONFIGURATION_LAYOUTDIR_RTL = 0x02;

			private const Byte MASK_LAYOUTDIR = 0xC0;
			private const Byte SHIFT_LAYOUTDIR = 6;

			private const Byte LAYOUTDIR_ANY = ACONFIGURATION_LAYOUTDIR_ANY << SHIFT_LAYOUTDIR;
			private const Byte LAYOUTDIR_LTR = ACONFIGURATION_LAYOUTDIR_LTR << SHIFT_LAYOUTDIR;
			private const Byte LAYOUTDIR_RTL = ACONFIGURATION_LAYOUTDIR_RTL << SHIFT_LAYOUTDIR;
			#endregion screenLayout bits for layout direction
			

			/// <summary>Number of bytes in this structure</summary>
			public Int32 size;						// uint32
			/// <summary>Mobile country code (from SIM). "0" means any.</summary>
			public Int16 mmc;						// uint16

			/// <summary>Mobile network code (from SIM). "0" means any.</summary>
			public Int16 mnc;						// uint16
			/// <summary>\0\0 means "any". Otherwise, en, fr, etc.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public Char[] language;
			/// <summary>\0\0 means "any". Otherwise, US, CA, etc.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public Char[] country;

			/// <summary>Orientation</summary>
			public Byte orientation;				// uint8

			/// <summary>TouchScreen</summary>
			public Byte touchscreen;				// uint8

			/// <summary>Density</summary>
			public Int16 density;					// uint16

			/// <summary>Keyboard</summary>
			public Byte keyboard;					// uint8

			/// <summary>Navigation</summary>
			public Byte navigation;					// uint8

			/// <summary>Input flags</summary>
			public Byte inputFlags;					// uint8

			/// <summary>Input padding0</summary>
			public Byte inputPad0;					// uint8

			/// <summary>Screen with</summary>
			public UInt16 screenWidth;				// uint16

			/// <summary>Screen height</summary>
			public UInt16 screenHeight;				// uint16

			/// <summary>SDK version</summary>
			public UInt16 sdkVersion;				// uint16

			/// <summary>Minor version</summary>
			public UInt16 minorVersion;				// uint16

			/// <summary>Screen layout</summary>
			public Byte screenLayout;				// uint8

			/// <summary>UI mode</summary>
			public Byte uiMode;						// uint8

			/// <summary>Smallest screen DP</summary>
			public UInt16 smallestScreenWidthDp;	// uint16

			/// <summary>Screen width DP</summary>
			public UInt16 screenWidthDp;			// uint16

			/// <summary>Screen height DP</summary>
			public UInt16 screenHeightDp;		// uint16

			/// <summary>Screen layout direction</summary>
			public String ScreenLayoutDirection
			{
				get
				{
					switch(this.screenLayout & MASK_LAYOUTDIR)
					{
					case LAYOUTDIR_LTR:
						return "ldltr";
					case LAYOUTDIR_RTL:
						return "ldrtl";
					default:
						return "any";
					}
				}
			}
		}

		/// <summary>This is the beginning of information about an entry in the resource table</summary>
		/// <remarks>
		/// It holds the reference to the name of this entry, and is immediately followed by one of:
		/// A Res_value structure, if FLAG_COMPLEX is -not- set.
		/// An array of ResTable_map structures, if FLAG_COMPLEX is set.
		/// These supply a set of name/value mappings of data.
		/// </remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct ResTable_entry
		{
			/// <summary>If set, this is a complex entry, holding a set of name/value mappings.</summary>
			/// <remarks>It is followed by an array of ResTable_map structures.</remarks>
			private const Int32 FLAG_COMPLEX = 0x0001;
			/// <summary>If set, this resource has been declared public, so libraries are allowed to reference it.</summary>
			private const Int32 FLAG_PUBLIC = 0x0002;

			/// <summary>Number of bytes in this structure.</summary>
			public Int16 size;

			/// <summary>Type of the entry (complex, simple, public,...)</summary>
			private Int16 flags;

			/// <summary>Reference into ResTable_package::keyStrings identifying this entry.</summary>
			public ResStringPool_ref key;

			/// <summary>If set, this is a complex entry, holding a set of name/value mappings.</summary>
			public Boolean IsComplex { get { return (this.flags & FLAG_COMPLEX) != 0; } }

			/// <summary>If set, this resource has been declared public, so libraries are allowed to reference it.</summary>
			public Boolean IsPublic { get { return (this.flags & FLAG_PUBLIC) == 1; } }
		}

		/// <summary>String pool descriptor</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct ResStringPool_ref
		{
			/// <summary>
			/// Index into the string pool table
			/// (uint32_t-offset from the indices immediately after ResStringPool_header)
			/// at which to find the location of the string data in the pool.
			/// </summary>
			public Int32 index;
		}

		/// <summary>A specification of the resources defined by a particular type.</summary>
		/// <remarks>
		/// There should be one of these chunks for each resource type.
		/// 
		/// This structure is followed by an array of integers providing the set of
		/// configuration change flags (ResTable_config::CONFIG_*) that have multiple resources for that configuration.
		/// In addition, the high bit is set if that resource has been made public.
		/// </remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct ResTable_typeSpec
		{
			/// <summary>Additional flag indicating an entry is public.</summary>
			public const Int32 SPEC_PUBLIC = 0x40000000;

			/// <summary>chunk header</summary>
			public ResChunk_Header header;

			/// <summary>The type identifier this chunk is holding.</summary>
			/// <remarks>
			/// Type IDs start at 1 (corresponding to the value of the type bits in a resource identifier).
			/// 0 is invalid.
			/// </remarks>
			public Byte id;

			/// <summary>Must be zero</summary>
			public Byte res0;

			/// <summary>Must be zero</summary>
			public Int16 res1;

			/// <summary>Number of uint32_t entry configuration masks that follow.</summary>
			public Int32 entryCount;

			/// <summary>Structure integrity check</summary>
			public Boolean IsValid { get { return this.res0 == 0 && this.res1 == 0; } }
		}

		/// <summary>Type of data stored in resources</summary>
		public enum DATA_TYPE : byte
		{
			/// <summary>Contains no data</summary>
			NULL = 0x00,
			/// <summary>ResTable_ref, a reference to another resource table entry</summary>
			REFERENCE = 0x01,
			/// <summary>Attribute resource identifier</summary>
			ATTRIBUTE = 0x02,
			/// <summary>Index into the containing resource table's global value string pool</summary>
			STRING = 0x03,
			/// <summary>Single-precision floating point number</summary>
			FLOAT = 0x04,
			/// <summary>Complex number encoding a dimension value, such as "100in"</summary>
			DIMENSION = 0x05,
			/// <summary>Complex number encoding a fraction of a container</summary>
			FRACTION = 0x06,

			/// <summary>Raw integer value of the form n..n</summary>
			INT_DEC = 0x10,
			/// <summary>Raw integer value of the form 0xn..n</summary>
			INT_HEX = 0x11,
			/// <summary>0 or 1, for input "false" or "true" respectively</summary>
			INT_BOOLEAN = 0x12,

			/// <summary>Raw integer value of the form #aarrggbb.</summary>
			INT_COLOR_ARGB8 = 0x1c,
			/// <summary>Raw integer value of the form #rrggbb.</summary>
			INT_COLOR_RGB8 = 0x1d,
			/// <summary>Raw integer value of the form #argb.</summary>
			INT_COLOR_ARGB4 = 0x1e,
			/// <summary>Raw integer value of the form #rgb.</summary>
			INT_COLOR_RGB4 = 0x1f,
		}

		/// <summary>Resource value</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Res_value
		{
			/// <summary>Complex data: bit location of unit information.</summary>
			private const Int32 COMPLEX_UNIT_SHIFT = 0;

			/// <summary>
			/// Complex data: mask to extract unit information (after shifting by <see cref="COMPLEX_UNIT_SHIFT"/>.
			/// This gives us 16 possible types, as defined below.
			/// </summary>
			private const Int32 COMPLEX_UNIT_MASK = 0xf;

			/// <summary>Complex data: bit location of mantissa information.</summary>
			private const Int32 COMPLEX_MANTISSA_SHIFT = 8;

			/// <summary>
			/// Complex data: mask to extract mantissa information (after shifting by <see cref="COMPLEX_MANTISSA_SHIFT"/>.
			/// This gives us 23 bits of precision; the top bit is the sign.
			/// </summary>
			private const Int32 COMPLEX_MANTISSA_MASK = 0xffffff;

			/// <summary>Complex data: where the radix information is, telling where the decimal place appears in the mantissa.</summary>
			private const Int32 COMPLEX_RADIX_SHIFT = 4;

			/// <summary>
			/// Complex data: mask to extract radix information (after shifting by <see cref="COMPLEX_RADIX_SHIFT"/>).
			/// This give us 4 possible fixed point representations as defined below.
			/// </summary>
			private const Int32 COMPLEX_RADIX_MASK = 0x3;

			private static String[] DIMENSION_UNIT_STRS = new String[] { "px", "dip", "sp", "pt", "in", "mm", };
			private static String[] FRACTION_UNIT_STRS = new String[] { "%", "%p", };

			private static Single MANTISSA_MULT = 1.0f / (1 << Res_value.COMPLEX_MANTISSA_SHIFT);

			private static Single[] RADIX_MULTS = new Single[] {
				1.0f*Res_value.MANTISSA_MULT, 1.0f/(1<<7)*Res_value.MANTISSA_MULT,
				1.0f/(1<<15)*Res_value.MANTISSA_MULT, 1.0f/(1<<23)*Res_value.MANTISSA_MULT
			};

			/// <summary>Number of bytes in this structure.</summary>
			public Int16 size;

			/// <summary>Always set to 0.</summary>
			public Byte res0;

			/// <summary>Type of the data value.</summary>
			public DATA_TYPE dataType;

			/// <summary>The data for this item, as interpreted according to dataType.</summary>
			public Int32 data;

			/// <summary>Convert resource value to String</summary>
			/// <returns>String representation of the value</returns>
			public String DataToString()
			{
				switch(this.dataType)
				{
				case DATA_TYPE.NULL:
					return null;
				case DATA_TYPE.REFERENCE:
					return "@" + data;
				case DATA_TYPE.ATTRIBUTE:
					return "?" + data;
				case DATA_TYPE.FLOAT:
					Byte[] bytes = BitConverter.GetBytes(data);
					return BitConverter.ToSingle(bytes, 0).ToString();
				case DATA_TYPE.INT_BOOLEAN:
					return this.data != 0 ? "true" : "false";
				case DATA_TYPE.INT_HEX:
					return "0x" + this.data.ToString("X4");
				case DATA_TYPE.DIMENSION:
					Single dim = complexToFloat(data);
					return dim.ToString() + DIMENSION_UNIT_STRS[(this.data >> COMPLEX_UNIT_SHIFT) & COMPLEX_UNIT_MASK];
				case DATA_TYPE.FRACTION:
					Single fraction = complexToFloat(data) * 100;
					return fraction.ToString() + FRACTION_UNIT_STRS[(this.data >> COMPLEX_UNIT_SHIFT) & COMPLEX_UNIT_MASK];
				case DATA_TYPE.INT_COLOR_RGB8:
					return "#" + Res_value.ToColorRGB8(this.data);
				case DATA_TYPE.INT_COLOR_RGB4:
					return "#" + Res_value.ToColorRGB4(this.data);
				case DATA_TYPE.INT_COLOR_ARGB8:
					return "#" + Res_value.ToColorARGB8(this.data);
				default:
					return this.data.ToString();
				}
			}

			private static String ToColorRGB4(Int32 data)
			{
				String color = data.ToString("X");
				String r, g, b;
				r = color.Substring(0, 1);
				g = color.Substring(1, 1);
				b = color.Substring(2, 1);
				return "0" + r + "0" + g + "0" + b;
			}

			private static String ToColorRGB8(Int32 data)
			{
				String color = data.ToString("X");
				String r, g, b;
				r = color.Substring(0, 2);
				g = color.Substring(2, 2);
				b = color.Substring(4, 2);
				return r + g + b;
			}

			private static String ToColorARGB8(Int32 data)
			{
				String color = data.ToString("X8");
				String a, r, g, b;
				a = color.Substring(0, 2);
				r = color.Substring(2, 2);
				g = color.Substring(4, 2);
				b = color.Substring(6, 2);
				return a + r + g + b;
			}

			/// <summary>Retrieve the base value from a complex data integer.</summary>
			/// <remarks>
			/// This uses the * <see cref="COMPLEX_MANTISSA_MASK"/> and <see cref="COMPLEX_RADIX_MASK"/> fields of
			/// the data to compute a floating point representation of the number they describe.
			/// The units are ignored.
			/// </remarks>
			/// <param name="complex">A complex data value.</param>
			/// <returns>A floating point value corresponding to the complex data.</returns>
			private static Single complexToFloat(Int32 complex)
			{
				return (complex & (Res_value.COMPLEX_MANTISSA_MASK
						<< Res_value.COMPLEX_MANTISSA_SHIFT))
						* RADIX_MULTS[(complex >> Res_value.COMPLEX_RADIX_SHIFT)
						& Res_value.COMPLEX_RADIX_MASK];
			}
		}

		/// <summary>
		/// This is a reference to a unique entry (a <see cref="ResTable_entry"/> structure) in a resource table.
		/// </summary>
		/// <remarks>
		/// The value is structured as:
		/// 
		/// 0xpptteeee,
		/// 
		/// where pp is the package index,
		/// tt is the type index in that package,
		/// and eeee is the entry index in that type.
		/// 
		/// The package and type values start at 1 for the first item, to help catch cases where they have not been supplied.
		/// </remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct ResTable_ref
		{
			/// <summary>banana banana banana</summary>
			public Int32 ident;
		}

		/// <summary>Extended form of a ResTable_entry for map entries, defining a parent map resource from which to inherit values.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct ResTable_map_entry
		{
			/// <summary>Resource identifier of the parent mapping, or 0 if there is none.</summary>
			public ResTable_ref parent;

			/// <summary>Number of name/value pairs that follow for FLAG_COMPLEX.</summary>
			public Int32 count;
		}

		/// <summary>A single name/value mapping that is part of a complex resource entry</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct ResTable_map
		{
			/// <summary>The resource identifier defining this mapping's name</summary>
			/// <remarks>
			/// For attribute resources, 'name' can be one of the following special resource types to supply meta-data about the attribute;
			/// for all other resource types it must be an attribute resource.
			/// </remarks>
			public ResTable_ref name;

			/// <summary>This mapping's value.</summary>
			public Res_value value;
		}
	}
}