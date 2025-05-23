using System;

namespace AlphaOmega.Debug
{
    public static class ResourceValueDecoder
    {
        /// <summary>Type of data</summary>
		public enum ValueType
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

        public static String DataToString(Int32 data, ValueType type)
        {
            switch(type)
            {
            case ValueType.NULL:
                return "";
            case ValueType.REFERENCE:
                return "@" + data;
            case ValueType.ATTRIBUTE:
                return "?" + data;
            case ValueType.FLOAT:
                Byte[] bytes = BitConverter.GetBytes(data);
                return BitConverter.ToSingle(bytes, 0).ToString();
            case ValueType.INT_BOOLEAN:
                return data != 0 ? "true" : "false";
            case ValueType.INT_HEX:
                return "0x" + data.ToString("X4");
            case ValueType.DIMENSION:
                Single dim = complexToFloat(data);
                return dim.ToString() + DIMENSION_UNIT_STRS[(data >> COMPLEX_UNIT_SHIFT) & COMPLEX_UNIT_MASK];
            case ValueType.FRACTION:
                Single fraction = complexToFloat(data) * 100;
                return fraction.ToString() + FRACTION_UNIT_STRS[(data >> COMPLEX_UNIT_SHIFT) & COMPLEX_UNIT_MASK];
            case ValueType.INT_COLOR_RGB8:
                return "#" + ToColorRGB8(data);
            case ValueType.INT_COLOR_RGB4:
                return "#" + ToColorRGB4(data);
            case ValueType.INT_COLOR_ARGB8:
                return "#" + ToColorARGB8(data);
            default:
                return data.ToString();
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