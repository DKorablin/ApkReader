using System;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>Class structure list</summary>
	public class class_data_row : BaseRow
	{
		internal enum columns
		{
			static_fields_size = 0,
			instance_fields_size = 1,
			direct_methods_size = 2,
			virtual_methods_size = 3,
			static_fields = 4,
			instance_fields = 5,
			direct_methods = 6,
			virtual_methods = 7,
		}

		/// <summary>The number of static fields defined in this item</summary>
		public Int32 static_fields_size { get { return base.GetValue<Int32>((UInt16)columns.static_fields_size); } }

		/// <summary>The number of instance fields defined in this item</summary>
		public Int32 instance_fields_size { get { return base.GetValue<Int32>((UInt16)columns.instance_fields_size); } }

		/// <summary>The number of direct methods defined in this item</summary>
		public Int32 direct_methods_size { get { return base.GetValue<Int32>((UInt16)columns.direct_methods_size); } }

		/// <summary>The number of virtual methods defined in this item</summary>
		public Int32 virtual_methods_size { get { return base.GetValue<Int32>((UInt16)columns.virtual_methods_size); } }

		private UInt32[] static_fieldsI { get { return base.GetValue<UInt32[]>((UInt16)columns.static_fields); } }

		/// <summary>The defined static fields, represented as a sequence of encoded elements.</summary>
		/// <remarks>The fields must be sorted by field_idx in increasing order.</remarks>
		public encoded_field_row[] static_fields
		{
			get
			{
				var table = base.File.encoded_field;
				return Array.ConvertAll(this.static_fieldsI, delegate(UInt32 rowIndex) { return table[rowIndex]; });
			}
		}

		private UInt32[] instance_fieldsI { get { return base.GetValue<UInt32[]>((UInt16)columns.instance_fields); } }

		/// <summary>The defined instance fields, represented as a sequence of encoded elements.</summary>
		/// <remarks>The fields must be sorted by field_idx in increasing order.</remarks>
		public encoded_field_row[] instance_fields
		{
			get
			{
				var table = base.File.encoded_field;
				return Array.ConvertAll(this.instance_fieldsI, delegate(UInt32 rowIndex) { return table[rowIndex]; });
			}
		}

		private UInt32[] direct_methodsI { get { return base.GetValue<UInt32[]>((UInt16)columns.direct_methods); } }

		/// <summary>The defined direct (any of static, private, or constructor) methods, represented as a sequence of encoded elements.</summary>
		/// <remarks>The methods must be sorted by method_idx in increasing order.</remarks>
		public encoded_method_row[] direct_methods
		{
			get
			{
				var table = base.File.encoded_method;
				return Array.ConvertAll(this.direct_methodsI, delegate(UInt32 rowIndex) { return table[rowIndex]; });
			}
		}

		private UInt32[] virtual_methodsI { get { return base.GetValue<UInt32[]>((UInt16)columns.virtual_methods); } }

		/// <summary>
		/// The defined virtual (none of static, private, or constructor) methods, represented as a sequence of encoded elements.
		/// This list should not include inherited methods unless overridden by the class that this item represents.
		/// </summary>
		/// <remarks>
		/// The methods must be sorted by method_idx in increasing order.
		/// The method_idx of a virtual method must not be the same as any direct method.
		/// </remarks>
		public encoded_method_row[] virtual_methods
		{
			get
			{
				var table = base.File.encoded_method;
				return Array.ConvertAll(this.virtual_methodsI, delegate(UInt32 rowIndex) { return table[rowIndex]; });
			}
		}
	}
}