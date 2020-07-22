using System;
using System.Runtime.InteropServices;

namespace AlphaOmega.Debug
{
	/// <summary>API structs Android XML</summary>
	public static class AxmlApi
	{
		/// <summary>Chunk types</summary>
		public enum ChunkType : int
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
			/// <summary>String value</summary>
			String=0x03,
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
			public Boolean IsValid { get { return this.signature == 0x80003; } }

			/// <summary>ASCII Encoding used in</summary>
			public Boolean IsAsciiEncoding { get { return this.encoding == 0x00000100; } }
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
			public AttributeValue ValueType { get { return (AttributeValue)(this._valueType >> 24);/*Padding +3 bytes*/ } }
		}
	}
}