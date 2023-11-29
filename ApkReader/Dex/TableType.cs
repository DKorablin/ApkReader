using System;

namespace AlphaOmega.Debug.Dex
{
	/// <summary>Strongly typed dex table viewer</summary>
	public enum TableType : UInt16
	{
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
		/// <summary>Type identifiers list</summary>
		TYPE_LIST = 0x1001,
		/// <summary>banana banana banana</summary>
		ANNOTATIONS_DIRECTORY_ITEM = 0x2006,
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
		/// <summary>banana banana banana</summary>
		try_item = 0x3001,
		/// <summary>Bytecode try/catch address</summary>
		encoded_catch_handler_list = 0x3002,
		/// <summary>Type of the catch block</summary>
		encoded_type_addr_pair = 0x3003,

		/// <summary>Field type name and access modifieres</summary>
		encoded_field = 0x3004,
		/// <summary>Method type name and access modifiers</summary>
		encoded_method = 0x3005,

		/// <summary>banana banana banana</summary>
		field_annotation=0x3006,
		/// <summary>banana banana banana</summary>
		method_annotation = 0x3007,
		/// <summary>banana banana banana</summary>
		parameter_annotation = 0x3008,
	}
}