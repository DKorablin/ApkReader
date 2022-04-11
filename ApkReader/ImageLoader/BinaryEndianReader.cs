using System;
using System.IO;

namespace AlphaOmega.Debug
{
	/// <summary>Binary reader with endianness check</summary>
	public class BinaryEndianReader : BinaryReader
	{
		private readonly EndianHelper.Endian _endiannes;

		/// <summary>Create instance of the binary reader specifying required endianness and input stream</summary>
		/// <param name="endianness">Required endianness</param>
		/// <param name="input">Byte input stream</param>
		public BinaryEndianReader(Stream input, EndianHelper.Endian endianness)
			: base(input)
		{
			this._endiannes = endianness;
		}

		/// <summary>Creates reader for specific byte endianness</summary>
		/// <param name="endiannes">Big or Little endian</param>
		/// <param name="input">Byte array stream</param>
		/// <returns>BinaryReader</returns>
		public static BinaryReader CreateReader(EndianHelper.Endian endiannes, MemoryStream input)
		{
			if(BitConverter.IsLittleEndian == (endiannes == EndianHelper.Endian.Little))
				return new BinaryReader(input);
			else
				return new BinaryEndianReader(input, endiannes);
		}

		/// <summary>Reads decimal and swap bytes if needed</summary>
		/// <returns>Decimal</returns>
		public override Decimal ReadDecimal()
		{
			Byte[] bytes = base.ReadBytes(sizeof(Decimal));
			EndianHelper.AdjustEndianness(typeof(Decimal), bytes, this._endiannes);
			var i1 = BitConverter.ToInt32(bytes, 0);
			var i2 = BitConverter.ToInt32(bytes, 4);
			var i3 = BitConverter.ToInt32(bytes, 8);
			var i4 = BitConverter.ToInt32(bytes, 12);

			return new Decimal(new Int32[] { i1, i2, i3, i4, });
		}

		/// <summary>Reads double and swap bytes if needed</summary>
		/// <returns>Double</returns>
		public override Double ReadDouble()
		{
			Byte[] bytes = base.ReadBytes(sizeof(Double));
			EndianHelper.AdjustEndianness(typeof(Double), bytes, this._endiannes);

			return BitConverter.ToDouble(bytes, 0);
		}

		/// <summary>Reads Int16 and swap bytes if needed</summary>
		/// <returns>Int16</returns>
		public override Int16 ReadInt16()
		{
			Byte[] bytes = base.ReadBytes(sizeof(Int16));
			EndianHelper.AdjustEndianness(typeof(Int16), bytes, this._endiannes);

			return BitConverter.ToInt16(bytes, 0);
		}

		/// <summary>Reads Int32 and swap bytes if needed</summary>
		/// <returns>Int32</returns>
		public override Int32 ReadInt32()
		{
			Byte[] bytes = base.ReadBytes(sizeof(Int32));
			EndianHelper.AdjustEndianness(typeof(Int32), bytes, this._endiannes);

			return BitConverter.ToInt32(bytes, 0);
		}

		/// <summary>Reads Int64 and swap bytes if needed</summary>
		/// <returns>Int64</returns>
		public override Int64 ReadInt64()
		{
			Byte[] bytes = base.ReadBytes(sizeof(Int64));
			EndianHelper.AdjustEndianness(typeof(Int64), bytes, this._endiannes);

			return BitConverter.ToInt64(bytes, 0);
		}

		/// <summary>Reads Single and swap bytes if needed</summary>
		/// <returns>Single</returns>
		public override Single ReadSingle()
		{
			Byte[] bytes = base.ReadBytes(sizeof(Single));
			EndianHelper.AdjustEndianness(typeof(Single), bytes, this._endiannes);

			return BitConverter.ToSingle(bytes, 0);
		}

		/// <summary>Reads unsigned Int16 and swap bytes if needed</summary>
		/// <returns>UInt16</returns>
		public override UInt16 ReadUInt16()
		{
			Byte[] bytes = base.ReadBytes(sizeof(UInt16));
			EndianHelper.AdjustEndianness(typeof(UInt16), bytes, this._endiannes);

			return BitConverter.ToUInt16(bytes, 0);
		}

		/// <summary>Reads unsigned Int32 and swap bytes if needed</summary>
		/// <returns>UInt32</returns>
		public override UInt32 ReadUInt32()
		{
			Byte[] bytes = base.ReadBytes(sizeof(UInt32));
			EndianHelper.AdjustEndianness(typeof(UInt32), bytes, this._endiannes);

			return BitConverter.ToUInt32(bytes, 0);
		}

		/// <summary>Reads unsigned Int64 and swap bytes if needed</summary>
		/// <returns>UInt64</returns>
		public override UInt64 ReadUInt64()
		{
			Byte[] bytes = base.ReadBytes(sizeof(UInt64));
			EndianHelper.AdjustEndianness(typeof(UInt64), bytes, this._endiannes);

			return BitConverter.ToUInt64(bytes, 0);
		}
	}
}