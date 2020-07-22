using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AlphaOmega.Debug.Dex;

namespace AlphaOmega.Debug
{
	/// <summary>Dalvik image reader</summary>
	public class DexFile : IDisposable
	{
		private IImageLoader _loader;
		private readonly DexApi.header_item _header;
		private DexApi.map_item[] _map_list;
		private readonly Dictionary<TableType, Table> _sections = new Dictionary<TableType, Table>();

		/// <summary>Image loader interface</summary>
		public IImageLoader Loader { get { return this._loader; } }

		/// <summary>Header identifier and description</summary>
		public DexApi.header_item header { get { return this._header; } }

		private Dictionary<TableType, Table> Sections { get { return this._sections; } }

		/// <summary>
		/// This is a list of the entire contents of a file, in order.
		/// It contains some redundancy with respect to the header_item but is intended to be an easy form to use to iterate over an entire file.
		/// A given type must appear at most once in a map, but there is no restriction on what order types may appear in, other than the restrictions implied by the rest of the format (e.g., a header section must appear first, followed by a string_ids section, etc.).
		/// Additionally, the map entries must be ordered by initial offset and must not overlap.
		/// </summary>
		public DexApi.map_item[] map_list
		{
			get
			{
				return this._map_list == null
					? this._map_list = this.ReadMapList()
					: this._map_list;
			}
		}

		/// <summary>
		/// String identifiers list.
		/// These are identifiers for all the strings used by this file, either for internal naming (e.g., type descriptors) or as constant objects referred to by code.
		/// </summary>
		/// <remarks>This list must be sorted by string contents, using UTF-16 code point values (not in a locale-sensitive manner), and it must not contain any duplicate entries.</remarks>
		public BaseTable<Dex.Tables.string_id_row> STRING_ID_ITEM
		{
			get { return new BaseTable<Dex.Tables.string_id_row>(this, TableType.STRING_ID_ITEM); }
		}

		/// <summary>
		/// String identifiers list.
		/// These are identifiers for all the strings used by this file, either for internal naming (e.g., type descriptors) or as constant objects referred to by code.
		/// </summary>
		/// <remarks>This list must be sorted by string contents, using UTF-16 code point values (not in a locale-sensitive manner), and it must not contain any duplicate entries.</remarks>
		public BaseTable<Dex.Tables.string_data_row> STRING_DATA_ITEM
		{
			get { return new BaseTable<Dex.Tables.string_data_row>(this, TableType.STRING_DATA_ITEM); }
		}

		/// <summary>Source bytecode payload</summary>
		public BaseTable<Dex.Tables.code_row> CODE_ITEM
		{
			get { return new BaseTable<Dex.Tables.code_row>(this, TableType.CODE_ITEM); }
		}

		/// <summary>
		/// Type identifiers list.
		/// These are identifiers for all types (classes, arrays, or primitive types) referred to by this file, whether defined in the file or not.
		/// </summary>
		/// <remarks>This list must be sorted by string_id index, and it must not contain any duplicate entries.</remarks>
		public BaseTable<Dex.Tables.type_id_row> TYPE_ID_ITEM
		{
			get { return new BaseTable<Dex.Tables.type_id_row>(this, TableType.TYPE_ID_ITEM); }
		}

		/// <summary>Referenced from <see cref="Dex.Tables.class_def_row"/> and <see cref="Dex.Tables.proto_id_row"/></summary>
		public BaseTable<Dex.Tables.type_list_row> TYPE_LIST
		{
			get { return new BaseTable<Dex.Tables.type_list_row>(this, TableType.TYPE_LIST); }
		}

		/// <summary>
		/// Method prototype identifiers list.
		/// These are identifiers for all prototypes referred to by this file.
		/// </summary>
		/// <remarks>
		/// This list must be sorted in return-type (by type_id index) major order, and then by argument list (lexicographic ordering, individual arguments ordered by type_id index).
		/// The list must not contain any duplicate entries.
		/// </remarks>
		public BaseTable<Dex.Tables.proto_id_row> PROTO_ID_ITEM
		{
			get { return new BaseTable<Dex.Tables.proto_id_row>(this, TableType.PROTO_ID_ITEM); }
		}

		/// <summary>
		/// Field identifiers list.
		/// These are identifiers for all fields referred to by this file, whether defined in the file or not.
		/// </summary>
		/// <remarks>
		/// This list must be sorted, where the defining type (by type_id index) is the major order, field name (by string_id index) is the intermediate order, and type (by type_id index) is the minor order.
		/// The list must not contain any duplicate entries.
		/// </remarks>
		public BaseTable<Dex.Tables.field_id_row> FIELD_ID_ITEM
		{
			get { return new BaseTable<Dex.Tables.field_id_row>(this, TableType.FIELD_ID_ITEM); }
		}

		/// <summary>
		/// Method identifiers list.
		/// These are identifiers for all methods referred to by this file, whether defined in the file or not.
		/// </summary>
		/// <remarks>
		/// This list must be sorted, where the defining type (by type_id index) is the major order, method name (by string_id index) is the intermediate order, and method prototype (by proto_id index) is the minor order.
		/// The list must not contain any duplicate entries.
		/// </remarks>
		public BaseTable<Dex.Tables.method_id_row> METHOD_ID_ITEM
		{
			get { return new BaseTable<Dex.Tables.method_id_row>(this, TableType.METHOD_ID_ITEM); }
		}

		/// <summary>Class structure list</summary>
		public BaseTable<Dex.Tables.class_data_row> CLASS_DATA_ITEM
		{
			get { return new BaseTable<Dex.Tables.class_data_row>(this, TableType.CLASS_DATA_ITEM); }
		}

		/// <summary>Static and instance fields from the class_data_item</summary>
		public BaseTable<Dex.Tables.encoded_field_row> encoded_field
		{
			get { return new BaseTable<Dex.Tables.encoded_field_row>(this, TableType.encoded_field); }
		}

		/// <summary>Class method definition list</summary>
		public BaseTable<Dex.Tables.encoded_method_row> encoded_method
		{
			get { return new BaseTable<Dex.Tables.encoded_method_row>(this, TableType.encoded_method); }
		}

		/// <summary>Class definitions list.</summary>
		/// <remarks>
		/// The classes must be ordered such that a given class's superclass and implemented interfaces appear in the list earlier than the referring class.
		/// Furthermore, it is invalid for a definition for the same-named class to appear more than once in the list.
		/// </remarks>
		public BaseTable<Dex.Tables.class_def_row> CLASS_DEF_ITEM
		{
			get { return new BaseTable<Dex.Tables.class_def_row>(this, TableType.CLASS_DEF_ITEM); }
		}

		/// <summary>Where in the code exceptions are caught and how to handle them.</summary>
		public BaseTable<Dex.Tables.try_item_row> try_item
		{
			get { return new BaseTable<Dex.Tables.try_item_row>(this, TableType.try_item); }
		}

		/// <summary>Catch handler lists</summary>
		public BaseTable<Dex.Tables.encoded_catch_handler_row> encoded_catch_handler_list
		{
			get { return new BaseTable<Dex.Tables.encoded_catch_handler_row>(this, TableType.encoded_catch_handler_list); }
		}

		/// <summary>One for each caught type, in the order that the types should be tested.</summary>
		public BaseTable<Dex.Tables.encoded_type_addr_pair_row> encoded_type_addr_pair
		{
			get { return new BaseTable<Dex.Tables.encoded_type_addr_pair_row>(this, TableType.encoded_type_addr_pair); }
		}

		/// <summary>banana banana banana</summary>
		public BaseTable<Dex.Tables.annotation_directory_row> ANNOTATIONS_DIRECTORY_ITEM
		{
			get { return new BaseTable<Dex.Tables.annotation_directory_row>(this, TableType.ANNOTATIONS_DIRECTORY_ITEM); }
		}

		/// <summary>banana banana banana</summary>
		public BaseTable<Dex.Tables.field_annotation_row> field_annotation
		{
			get { return new BaseTable<Dex.Tables.field_annotation_row>(this, TableType.field_annotation); }
		}

		/// <summary>banana banana banana</summary>
		public BaseTable<Dex.Tables.method_annotation_row> method_annotation
		{
			get { return new BaseTable<Dex.Tables.method_annotation_row>(this, TableType.method_annotation); }
		}

		/// <summary>banana banana banana</summary>
		public BaseTable<Dex.Tables.parameter_annotation_row> parameter_annotation
		{
			get { return new BaseTable<Dex.Tables.parameter_annotation_row>(this, TableType.parameter_annotation); }
		}

		/// <summary>banana banana banana</summary>
		public BaseTable<Dex.Tables.annotation_set_ref_row> ANNOTATION_SET_REF_LIST
		{
			get { return new BaseTable<Dex.Tables.annotation_set_ref_row>(this, TableType.ANNOTATION_SET_REF_LIST); }
		}

		/// <summary>banana banana banana</summary>
		public BaseTable<Dex.Tables.annotation_set_row> ANNOTATION_SET_ITEM
		{
			get { return new BaseTable<Dex.Tables.annotation_set_row>(this, TableType.ANNOTATION_SET_ITEM); }
		}

		/// <summary>Create instance of the DEX</summary>
		/// <param name="loader">Loader type</param>
		public DexFile(IImageLoader loader)
		{
			if(loader == null)
				throw new ArgumentNullException("loader");

			this._loader = loader;
			this._loader.Endianness = EndianHelper.Endian.Little;

			this._header = this.PtrToStructure<DexApi.header_item>(0);
			if(!this._header.IsValid)
				throw new InvalidOperationException("Invalid DEX header");

			switch(this.header.endian_tag)
			{
			case DexApi.ENDIAN.ENDIAN:
				this._loader.Endianness = EndianHelper.Endian.Little;
				break;
			case DexApi.ENDIAN.REVERSE_ENDIAN:
				this._loader.Endianness = EndianHelper.Endian.Big;
				this._header = this.PtrToStructure<DexApi.header_item>(0);//TODO: Check it
				break;
			}
		}

		/// <summary>Get structure from specific RVA</summary>
		/// <typeparam name="T">Structure to map</typeparam>
		/// <param name="offset">RVA to the beggining of structure</param>
		/// <returns>Mapped structure</returns>
		public T PtrToStructure<T>(UInt32 offset) where T : struct
		{
			return this.Loader.PtrToStructure<T>(offset);
		}

		/// <summary>Get string from specific RVA</summary>
		/// <param name="offset">RVA to the beggining of string</param>
		/// <returns>Mapped string</returns>
		public String PtrToStringAnsi(UInt32 offset)
		{
			return this.Loader.PtrToStringAnsi(offset);
		}

		/// <summary>Read signed integer from image</summary>
		/// <param name="offset">Offset from the beggining of the stream</param>
		/// <returns>Result</returns>
		internal Int32 ReadSLeb128(ref UInt32 offset)
		{//https://android.googlesource.com/platform/libcore/+/master/dex/src/main/java/com/android/dex/Leb128.java
			Int32 result = 0;
			Int32 current;
			Int32 count = 0;
			Int32 signBits = -1;

			do
			{
				current = this.Loader.ReadBytes(offset, 1)[0];
				offset++;

				result |= (current & 0x7f) << (count * 7);
				signBits <<= 7;
				count++;
			} while(((current & 0x80) == 0x80) && count < 5);

			if((current & 0x80) == 0x80)
				throw new InvalidOperationException("Invalid LEB128 sequence");

			if(((signBits >> 1) & result) != 0)
				result |= signBits;

			return result;
		}

		/// <summary>Reads in a 32-bit integer in compressed format.</summary>
		/// <param name="offset">Offset from the beggining of the stream</param>
		/// <exception cref="InvalidOperationException">Invalid LEB128 data</exception>
		/// <remarks>BinaryReader.Read7BitEncodedInt</remarks>
		/// <returns>A 32-bit integer in compressed format.</returns>
		internal Int32 ReadULeb128(ref UInt32 offset)
		{//https://android.googlesource.com/platform/libcore/+/master/dex/src/main/java/com/android/dex/Leb128.java
			Int32 result = 0;
			Int32 current;
			Int32 count = 0;

			do
			{
				current = this.Loader.ReadBytes(offset, 1)[0];
				offset++;

				result |= (current & 0x7f) << (count * 7);
				count++;
			} while((current & 0x80) == 0x80 && count < 5);

			if((current & 0x80) == 0x80)
				throw new InvalidOperationException("Invalid LEB128 sequence");

			return result;
		}

		/// <summary>Get required section table</summary>
		/// <param name="type">Type of thre required section header</param>
		/// <returns>Section header or null</returns>
		public DexApi.map_item? GetMapItem(DexApi.TYPE type)
		{
			foreach(DexApi.map_item item in this.map_list)
				if(item.type == type)
					return item;

			return null;//TODO: Check for empty section
		}

		private T GetMapItemT<T>(DexApi.TYPE type) where T : Table, new()
		{
			DexApi.map_item? mapItem = this.GetMapItem(type);
			if(mapItem == null)
				return null;

			T result = new T();
			result.ReadSection(this, mapItem.Value.offset, mapItem.Value.size);
			return result;
		}

		/// <summary>Get all known section tables</summary>
		/// <returns>Loaded and cached section tables</returns>
		public IEnumerable<Table> GetSectionTables()
		{
			foreach(TableType type in Enum.GetValues(typeof(TableType)))
			{
				Table section = this.GetSectionTable(type);
				if(section != null)
					yield return section;
			}
		}

		/// <summary>Get cached section table or load it from current DEX file</summary>
		/// <param name="type">Section type to get</param>
		/// <returns>Loaded section table from cache or DEX file</returns>
		public Table GetSectionTable(TableType type)
		{
			Table result;
			if(!this.Sections.TryGetValue(type, out result))
			{
				result = this.ReadSectionTable(type);

				this.Sections.Add(type, result);
			}
			return result;
		}

		/// <summary>Read section table from DEX file by offset</summary>
		/// <param name="type">Section type with required payload</param>
		/// <returns>Loaded table from DEX file</returns>
		public Table ReadSectionTable(TableType type)
		{
			Table result;
			switch(type)
			{
			case TableType.encoded_catch_handler_list:
				result = new TableEncodedCatchHandler();
				break;
			case TableType.try_item:
			case TableType.encoded_type_addr_pair:
			case TableType.encoded_field:
			case TableType.encoded_method:
			case TableType.field_annotation:
			case TableType.method_annotation:
			case TableType.parameter_annotation:
				result = new Table(type);
				break;
			case TableType.STRING_DATA_ITEM:
				result = this.GetMapItemT<TableString>((DexApi.TYPE)type);
				break;
			case TableType.CODE_ITEM:
				result = this.GetMapItemT<TableCode>((DexApi.TYPE)type);
				break;
			case TableType.CLASS_DATA_ITEM:
				result = this.GetMapItemT<TableClass>((DexApi.TYPE)type);
				break;
			case TableType.ANNOTATIONS_DIRECTORY_ITEM:
				result = this.GetMapItemT<TableAnnotationDirectory>((DexApi.TYPE)type);
				break;
			default:
				DexApi.map_item? mapItem = this.GetMapItem((DexApi.TYPE)type);
				if(mapItem == null)
					return null;

				result = new Table(type);
				result.ReadSection(this, mapItem.Value.offset, mapItem.Value.size);
				break;
			}

			return result;
		}

		private DexApi.map_item[] ReadMapList()
		{
			if(!this.header.IsValid)
				throw new InvalidOperationException("Invalid header");

			UInt32 sizeOfStruct = (UInt32)Marshal.SizeOf(typeof(DexApi.map_item));

			UInt32 offset = this.header.map_off;
			UInt32 size = this.PtrToStructure<UInt32>(this.header.map_off);
			offset += sizeof(UInt32);

			DexApi.map_item[] result = new DexApi.map_item[size];
			for(UInt32 loop = 0; loop < size; loop++)
			{
				result[loop] = this.PtrToStructure<DexApi.map_item>(offset);
				offset += sizeOfStruct;
			}

			return result;
		}

		/// <summary>dispose data reader and all managed resources</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Dispose managed objects</summary>
		/// <param name="disposing">Dispose managed objects</param>
		protected virtual void Dispose(Boolean disposing)
		{
			if(disposing && this._loader != null)
			{
				this._loader.Dispose();
				this._loader = null;
			}
		}
	}
}