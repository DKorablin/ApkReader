using System;
using System.Runtime.InteropServices;

namespace AlphaOmega.Debug
{
	/// <summary>API structures and constants for the Android Binary XML (AXML) format.</summary>
	public static class AxmlApi
	{
		/// <summary>Types of chunks found in an AXML file.</summary>
		public enum ChunkType : UInt16
		{
			/// <summary>Signalize that parser is at the very beginning of the document and nothing was read yet.</summary>
			RES_XML_START_NAMESPACE_TYPE = 0x0100,

			/// <summary>
			/// Logical end of the xml document.
			/// Returned from getEventType, next() and nextToken() when the end of the input document has been reached.
			/// </summary>
			RES_XML_END_NAMESPACE_TYPE = 0x0101,

			/// <summary>The name of start tag is available from getName(), its namespace and prefix are available from getNamespace() and getPrefix()</summary>
			RES_XML_START_ELEMENT_TYPE = 0x0102,

			/// <summary>The name of start tag is available from getName(), its namespace and prefix are available from getNamespace() and getPrefix().</summary>
			RES_XML_END_ELEMENT_TYPE = 0x0103,

			/// <summary>Character data was read and will is available by calling getText().</summary>
			RES_XML_CDATA_TYPE = 0x0104,

			/// <summary>Marker for the last valid XML tree chunk type.</summary>
			RES_XML_LAST_CHUNK_TYPE = 0x017f,

			/// <summary>Standard XML chunk aliases for PullParser compatibility.</summary>
			CData = 0x0105,

			/// <summary>An entity reference was just read</summary>
			EntityReference = 0x0106,

			/// <summary>Ignorable whitespace was just read</summary>
			Whitespace = 0x0107,

			/// <summary>An XML processing instruction declaration was just read</summary>
			ProcessingInstruction = 0x0108,

			/// <summary>An XML comment was just read</summary>
			Comment = 0x0109,

			/// <summary>An XML document type declaration was just read</summary>
			Declaration = 0x0110,

			/// <summary>Contains the mapping of string pool indices to Android Resource IDs.</summary>
			RES_XML_RESOURCE_MAP_TYPE = 0x0180,
		}

		/// <summary>Data types for typed values (Res_value) in attributes.</summary>
		public enum DataType : Byte
		{
			/// <summary>The value contains no data.</summary>
			NULL = 0x00,

			/// <summary>A reference to another resource ID (e.g., @id/button1).</summary>
			REFERENCE = 0x01,

			/// <summary>An attribute reference (e.g., ?android:attr/textColor).</summary>
			ATTRIBUTE = 0x02,

			/// <summary>String value</summary>
			STRING = 0x03,

			/// <summary>A 32-bit floating point number.</summary>
			FLOAT = 0x04,

			/// <summary>A dimension value (dp, px, sp) encoded in the data.</summary>
			DIMENSION = 0x05,

			/// <summary>A fraction or percentage.</summary>
			FRACTION = 0x06,

			/// <summary>A standard base-10 integer.</summary>
			INT_DEC = 0x10,

			/// <summary>A base-16 (hex) integer.</summary>
			INT_HEX = 0x11,

			/// <summary>0 for false, 0xFFFFFFFF for true.</summary>
			INT_BOOLEAN = 0x12,

			/// <summary>A hex color #AARRGGBB.</summary>
			INT_COLOR_ARGB8 = 0x1c,

			/// <summary>A hex color #RRGGBB.</summary>
			INT_COLOR_RGB8 = 0x1d,

			/// <summary>A hex color #ARGB.</summary>
			INT_COLOR_ARGB4 = 0x1e,

			/// <summary>A hex color #RGB.</summary>
			INT_COLOR_RGB4 = 0x1f,
		}

		/// <summary>Represents the binary header at the very start of an Android Binary XML file.</summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct AxmlFileHeader
		{
			/// <summary>Magic number identifying the file as AXML.</summary>
			/// <remarks>Usually 0x00080003.</remarks>
			public UInt32 MagicNumber;

			/// <summary>The total size of the binary XML file in bytes, including this header.</summary>
			public UInt32 FileSize;

			/// <summary>Represents the String Pool chunk header, which contains the table of all strings used within the AXML document.</summary>
			public StringPoolHeader StringPool;

			/// <summary>Checks if the file magic number matches the expected AXML signature.</summary>
			public Boolean IsValid => this.MagicNumber == 0x00080003;
		}

		/// <summary>Represents the String Pool chunk header, which contains the table of all strings used within the AXML document.</summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct StringPoolHeader
		{
			/// <summary>Chunk type signature (typically 0x001C0001 for String Pool).</summary>
			public UInt32 ChunkType;

			/// <summary>Total size of the string pool chunk (including header, offsets, and data).</summary>
			public UInt32 ChunkSize;

			/// <summary>Total number of strings contained in this pool.</summary>
			public UInt32 stringCount;

			/// <summary>Total number of style spans contained in this pool.</summary>
			public UInt32 StyleCount;

			/// <summary>Flags describing the encoding.</summary>
			/// <remarks>If (flags &amp; 0x100) != 0, strings are encoded in UTF-8; otherwise, UTF-16.</remarks>
			public UInt32 Flags;

			/// <summary>The byte offset from the start of this chunk to the beginning of the string data.</summary>
			public UInt32 stringsStart;

			/// <summary>The byte offset from the start of this chunk to the beginning of the style data.</summary>
			public UInt32 stylesStart;

			/// <summary>Returns true if the strings are stored using UTF-8 encoding.</summary>
			public Boolean IsUtf8 => (this.Flags & 0x00000100) != 0;
		}

		/// <summary>chunk structure</summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Chunk
		{
			/// <summary>Tag type</summary>
			public ChunkType TagType;

			/// <summary>Chunk length</summary>
			public Int16 HeaderSize;

			/// <summary>XML file line</summary>
			public UInt32 ChunkSize;
		}

		/// <summary>Data for a starting XML element (ResXMLTree_attrExt).</summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct StartTag
		{
			/// <summary>Commonly 0xFFFFFFFF, part of the ResXMLTree_node prefix.</summary>
			Int32 OxFFFFFFFF;

			/// <summary>Index into the string pool for the element name.</summary>
			public Int32 tagName;

			/// <summary>Attribute flags (unused in most standard AXML).</summary>
			public Int32 flags;

			/// <summary>The number of attributes associated with this tag.</summary>
			public Int16 attributeCount;

			/// <summary>Index of the 'id' attribute (1-based), or 0 if none.</summary>
			Int16 idIndex;

			/// <summary>Index of the 'class' attribute (1-based), or 0 if none.</summary>
			Int32 classIndex;
		}

		/// <summary>Data for an ending XML element (ResXMLTree_node).</summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct EndTag
		{
			/// <summary>Commonly 0xFFFFFFFF, part of the ResXMLTree_node prefix.</summary>
			Int32 OxFFFFFFFF;

			/// <summary>Index into the string pool for the element name.</summary>
			public Int32 tagName;
		}

		/// <summary>Namespace information (ResXMLTree_namespaceExt).</summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Document
		{
			/// <summary>Index into the string pool for the namespace prefix.</summary>
			public Int32 NamespaceIndex;

			/// <summary>Index into the string pool for the namespace URI.</summary>
			public Int32 name;
		}

		/// <summary>Raw text or CDATA node (ResXMLTree_cdataExt).</summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Text
		{
			/// <summary>Index into the string pool for the raw text content.</summary>
			public Int32 tagName;

			/// <summary>Typed data header for the text value.</summary>
			public Int32 typedData_size;

			/// <summary>Typed data value for the text content.</summary>
			public Int32 typedData_data;
		}

		/// <summary>Attribute entry within a StartTag (ResXMLTree_attribute).</summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Attribute
		{
			/// <summary>Index into string pool for the attribute namespace.</summary>
			public Int32 NamespaceIndex;

			/// <summary>Index into string pool for the attribute name.</summary>
			public Int32 NameIndex;

			/// <summary>Index into string pool for the raw string value (optional).</summary>
			public Int32 RawValueIndex;

			/// <summary>The size of the value. (Usually 8 bytes)</summary>
			public UInt16 ValueSize;

			/// <summary>Reserved padding, always 0.</summary>
			public Byte Res0;

			/// <summary>The data type of the attribute value.</summary>
			public DataType ValueType;

			/// <summary>The actual typed data (ID, color, boolean, or resource ID).</summary>
			public Int32 Data;
		}
	}
}