using System;
using System.Runtime.InteropServices;

namespace AlphaOmega.Debug
{
	/// <summary>API structs Android XML</summary>
	public static class AxmlApi
	{
		/// <summary>Chunk types</summary>
		public enum ChunkType : Int32
		{
			/// <summary>Signalize that parser is at the very beginning of the document and nothing was read yet.</summary>
			StartDocument = 0x00100100,
			/// <summary>
			/// Logical end of the xml document.
			/// Returned from getEventType, next() and nextToken() when the end of the input document has been reached.
			/// </summary>
			EndDocument = 0x00100101,
			/// <summary>The name of start tag is available from getName(), its namespace and prefix are available from getNamespace() and getPrefix()</summary>
			StartTag = 0x00100102,
			/// <summary>The name of start tag is available from getName(), its namespace and prefix are available from getNamespace() and getPrefix().</summary>
			EndTag = 0x00100103,
			/// <summary>Character data was read and will is available by calling getText().</summary>
			Text = 0x00100104,
			/// <summary>A CDATA sections was just read;</summary>
			CData = 0x00100105,
			/// <summary>An entity reference was just read</summary>
			EntityReference = 0x00100106,
			/// <summary>Ignorable whitespace was just read</summary>
			Whitespace = 0x00100107,
			/// <summary>An XML processing instruction declaration was just read</summary>
			ProcessingInstruction = 0x00100108,
			/// <summary>An XML comment was just read</summary>
			Comment = 0x00100109,
			/// <summary>An XML document type declaration was just read</summary>
			Declaration = 0x00100110,
		}

		/// <summary>Attibute value type</summary>
		public enum AttributeValue
		{
			NULL = 0x00,
			REFERENCE = 0x01,
			ATTRIBUTE = 0x02,
			STRING = 0x03,
			FLOAT = 0x04,
			DIMENSION = 0x05,
			FRACTION = 0x06,

			INT_DEC = 0x10,
			INT_HEX = 0x11,
			INT_BOOLEAN = 0x12,

			INT_COLOR_ARGB8 = 0x1c,
			INT_COLOR_RGB8 = 0x1d,
			INT_COLOR_ARGB4 = 0x1e,
			INT_COLOR_RGB4 = 0x1f,
		}

		/// <summary>AXML header file</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct AxmlHeader
		{
			/// <summary>AXML signature</summary>
			public Int32 signature;
			/// <summary>file size</summary>
			public Int32 FileSize;
			/// <summary>banana banana banana</summary>
			public Int32 chunkSignature;
			/// <summary>Offset to xml data</summary>
			public Int32 xmlOffset;
			/// <summary>String table count</summary>
			public Int32 stringCount;
			/// <summary>banana banana banana</summary>
			public Int32 unk1;
			/// <summary>String table encoding</summary>
			public Int32 encoding;
			/// <summary>banana banana banana</summary>
			public Int32 unk3;
			/// <summary>banana banana banana</summary>
			public Int32 unk4;

			/// <summary>Is AXML header valid</summary>
			public Boolean IsValid => this.signature == 0x80003;

			/// <summary>ASCII Encoding used in</summary>
			public Boolean IsAsciiEncoding => this.encoding == 0x00000100;
		}

		/// <summary>chunk structure</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Chunk
		{
			/// <summary>Tag type</summary>
			public ChunkType TagType;

			/// <summary>Chunk length</summary>
			public Int32 sourceLength;

			/// <summary>XML file line</summary>
			public Int32 sourceLine;

			/// <summary>Check</summary>
			Int32 OxFFFFFFFF;
		}

		/// <summary>Start tag description</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct StartTag
		{
			/// <summary>Check</summary>
			Int32 OxFFFFFFFF;

			/// <summary>Tag name index</summary>
			public Int32 tagName;

			/// <summary>banana banana banana</summary>
			public Int32 flags;

			/// <summary>Tag attibutes count</summary>
			public Int16 attributeCount;

			/// <summary>banana banana banana</summary>
			Int16 unk1;

			/// <summary>banana banana banana</summary>
			Int32 unk2;
		}

		/// <summary>End tag description</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct EndTag
		{
			/// <summary>Check</summary>
			Int32 OxFFFFFFFF;

			/// <summary>Tag name index</summary>
			public Int32 tagName;
		}

		/// <summary>Document tag description</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Document
		{
			/// <summary>Namespace index</summary>
			public Int32 @namespace;

			/// <summary>Name index</summary>
			public Int32 name;
		}

		/// <summary>Text tag description</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Text
		{
			/// <summary>Tag name index</summary>
			public Int32 tagName;

			/// <summary>banana banana banana</summary>
			public Int32 unk1;
			
			/// <summary>banana banana banana</summary>
			public Int32 unk2;
		}

		/// <summary>Attribute description</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Attribute
		{
			/// <summary>Namespace name index</summary>
			public Int32 @namespace;
			/// <summary>Name index</summary>
			public Int32 name;
			/// <summary>String value index</summary>
			public Int32 valueString;

			/// <summary>banana banana banana</summary>
			Int32 _valueType;

			/// <summary>Value</summary>
			public Int32 value;

			/// <summary>Value type</summary>
			public AttributeValue ValueType => (AttributeValue)(this._valueType >> 24);/*Padding +3 bytes*/

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

			private static Single MANTISSA_MULT = 1.0f / (1 << COMPLEX_MANTISSA_SHIFT);

			private static Single[] RADIX_MULTS = new Single[] {
				1.0f*MANTISSA_MULT, 1.0f/(1<<7)*MANTISSA_MULT,
				1.0f/(1<<15)*MANTISSA_MULT, 1.0f/(1<<23)*MANTISSA_MULT
			};

			public String DataToString()
			{
				switch(this.ValueType)
				{
				case AttributeValue.NULL:
					return "";
				case AttributeValue.REFERENCE:
					return "@" + value;
				case AttributeValue.ATTRIBUTE:
					return "?" + value;
				case AttributeValue.FLOAT:
					Byte[] bytes = BitConverter.GetBytes(value);
					return BitConverter.ToSingle(bytes, 0).ToString();
				case AttributeValue.INT_BOOLEAN:
					return this.value != 0 ? "true" : "false";
				case AttributeValue.INT_HEX:
					return "0x" + this.value.ToString("X4");
				case AttributeValue.DIMENSION:
					Single dim = complexToFloat(value);
					return dim.ToString() + DIMENSION_UNIT_STRS[(this.value >> COMPLEX_UNIT_SHIFT) & COMPLEX_UNIT_MASK];
				case AttributeValue.FRACTION:
					Single fraction = complexToFloat(value) * 100;
					return fraction.ToString() + FRACTION_UNIT_STRS[(this.value >> COMPLEX_UNIT_SHIFT) & COMPLEX_UNIT_MASK];
				case AttributeValue.INT_COLOR_RGB8:
					return "#" + ToColorRGB8(this.value);
				case AttributeValue.INT_COLOR_RGB4:
					return "#" + ToColorRGB4(this.value);
				case AttributeValue.INT_COLOR_ARGB8:
					return "#" + ToColorARGB8(this.value);
				default:
					return this.value.ToString();
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
				=> (complex & (COMPLEX_MANTISSA_MASK
					<< COMPLEX_MANTISSA_SHIFT))
					* RADIX_MULTS[(complex >> COMPLEX_RADIX_SHIFT)
					& COMPLEX_RADIX_MASK];
		}
	}
}