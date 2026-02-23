using System;
using System.Collections.Generic;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>Represents an annotation set reference list item in a DEX file.</summary>
	/// <remarks>
	/// An annotation set reference list is a variable-size array of offsets that reference
	/// individual annotation set items. This structure is used in parameter annotation contexts
	/// to specify which annotations apply to each parameter of a method.
	/// Each element in the list corresponds to a method parameter in order, with a zero offset
	/// indicating no annotations for that parameter.
	/// </remarks>
	public class annotation_set_ref_row : BaseRow
	{
		private UInt32[] annotations_offI => base.GetValue<UInt32[]>(0);

		/// <summary>
		/// Gets an enumerable collection of annotation sets referenced by this item.
		/// 
		/// Yields all non-zero <see cref="annotation_set_row"/> objects corresponding to the offsets in this annotation set reference list.
		/// Zero offsets (indicating no annotations for that parameter) are automatically filtered out.
		/// </summary>
		/// <remarks>
		/// Each offset in the list should either be zero (no annotations) or point to a location
		/// in the data section containing an <see cref="TableType.ANNOTATION_SET_ITEM"/> structure.
		/// The offset values are typically in increasing order and correspond to method parameters sequentially.
		/// </remarks>
		public IEnumerable<annotation_set_row> annotations_off
		{
			get
			{
				BaseTable<Dex.Tables.annotation_set_row> table = base.File.ANNOTATION_SET_ITEM;
				UInt32[] offsets = this.annotations_offI;
				for(UInt32 loop = 0; loop < offsets.Length; loop++)
					if(offsets[loop] > 0)
						yield return table.GetRowByOffset(offsets[loop]);
			}
		}
	}
}