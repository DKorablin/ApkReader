using System;
using System.Collections.Generic;

namespace AlphaOmega.Debug.Dex.Tables
{
	/// <summary>banana banana banana</summary>
	public class annotation_set_ref_row : BaseRow
	{
		private UInt32[] annotations_offI { get { return base.GetValue<UInt32[]>(0); } }

		/// <summary>Offset from the start of the file to the referenced annotation set or 0 if there are no annotations for this element.</summary>
		/// <remarks>
		/// The offset, if non-zero, should be to a location in the data section.
		/// The format of the data is specified by "annotation_set_item" below.
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