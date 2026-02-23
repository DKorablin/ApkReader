using System;

namespace AlphaOmega.Debug.Dex
{
	/// <summary>Strongly typed dex table viewer</summary>
	public enum TableType : UInt16
	{
		/// <summary>String identifiers list. Contains offsets to the string data.</summary>
		STRING_ID_ITEM = 0x0001,
		/// <summary>Type identifiers list.</summary>
		/// <remarks>Contains indices into the string identifier list.</remarks>
		TYPE_ID_ITEM = 0x0002,
		/// <summary>Method prototype identifiers list.</summary>
		/// <remarks>Defines return types and parameters.</remarks>
		PROTO_ID_ITEM = 0x0003,
		/// <summary>Field identifiers list.</summary>
		/// <remarks>Defines the class, type, and name of fields.</remarks>
		FIELD_ID_ITEM = 0x0004,
		/// <summary>Method identifiers list.</summary>
		/// <remarks>Defines the class, prototype, and name of methods.</remarks>
		METHOD_ID_ITEM = 0x0005,
		/// <summary>Class definitions list.</summary>
		/// <remarks>The main hub for class hierarchy and data pointers.</remarks>
		CLASS_DEF_ITEM = 0x0006,
		/// <summary>Type identifiers list.</summary>
		/// <remarks>Represents a list of types, typically used for method parameters or interfaces.</remarks>
		TYPE_LIST = 0x1001,
		/// <summary>Annotation directory items - contains all annotations for a class.</summary>
		/// <remarks>
		/// Annotation directory items serve as indices to locate all annotations related to
		/// a specific class definition, including class-level, field-level, method-level,
		/// and parameter-level annotations.
		/// </remarks>
		ANNOTATIONS_DIRECTORY_ITEM = 0x2006,
		/// <summary>Annotation set reference list - array of annotation set item offsets.</summary>
		/// <remarks>
		/// Annotation set reference lists are used in parameter annotation contexts to
		/// specify which annotations apply to each method parameter.
		/// </remarks>
		ANNOTATION_SET_REF_LIST = 0x1002,
		/// <summary>Annotation set items - collections of annotations for a single element.</summary>
		/// <remarks>
		/// Annotation set items contain annotations applied to a specific element
		/// (class, field, method, or parameter).
		/// </remarks>
		ANNOTATION_SET_ITEM = 0x1003,
		/// <summary>Class structure list. Contains static/instance fields and direct/virtual methods.</summary>
		CLASS_DATA_ITEM = 0x2000,
		/// <summary>Source bytecode payload. Contains the actual instructions, register size, and stack info.</summary>
		CODE_ITEM = 0x2001,
		/// <summary>String data items. Contains the actual UTF-8 encoded string content.</summary>
		STRING_DATA_ITEM = 0x2002,
		/// <summary>Exception handler structure. Defines the range of code covered by a try block.</summary>
		try_item = 0x3001,
		/// <summary>List of catch handlers.</summary>
		/// <remarks>Maps exception types to bytecode addresses.</remarks>
		encoded_catch_handler_list = 0x3002,
		/// <summary>A pair representing an exception type and the address of its handler.</summary>
		encoded_type_addr_pair = 0x3003,

		/// <summary>Field definition including its index and access flags (public, static, etc.).</summary>
		encoded_field = 0x3004,
		/// <summary>Method definition including its index, access flags, and code offset.</summary>
		encoded_method = 0x3005,

		/// <summary>Field annotation items - associates annotations with specific fields.</summary>
		/// <remarks>
		/// Field annotation items link annotations to individual fields in a class,
		/// specifying which field is annotated and providing the annotation set location.
		/// </remarks>
		field_annotation = 0x3006,
		/// <summary>Method annotation items - associates annotations with specific methods.</summary>
		/// <remarks>
		/// Method annotation items link annotations to individual methods in a class,
		/// specifying which method is annotated and providing the annotation set location.
		/// </remarks>
		method_annotation = 0x3007,
		/// <summary>Parameter annotation items - associates annotations with method parameters.</summary>
		/// <remarks>
		/// Parameter annotation items link annotations to individual method parameters,
		/// specifying which method's parameters are annotated and providing the
		/// annotation set reference list location.
		/// </remarks>
		parameter_annotation = 0x3008,
	}
}