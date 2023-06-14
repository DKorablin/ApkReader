using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace AlphaOmega.Debug
{
	internal static class Utils
	{
		/// <summary>Get structure from specific padding from the beginning of the image</summary>
		/// <typeparam name="T">Structure type</typeparam>
		/// <param name="reader">binaryreader</param>
		/// <returns>Readed structure from image</returns>
		internal static T PtrToStructure<T>(BinaryReader reader) where T : struct
		{
			Byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

			return PtrToStructure<T>(bytes);
		}

		internal static T PtrToStructure<T>(Byte[] payload) where T : struct
		{
			GCHandle handle = GCHandle.Alloc(payload, GCHandleType.Pinned);
			try
			{
				T result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
				return result;
			} finally
			{
				handle.Free();
			}
		}

		internal delegate void AppendToDictionaryDelegate<T>(T value);

		internal static void AppendToDictionary<K, V>(Dictionary<K, V> dictionary, K key, AppendToDictionaryDelegate<V> d) where V:new()
		{
			V value;
			if(!dictionary.TryGetValue(key, out value))
				dictionary.Add(key, value=new V());

			d(value);
			dictionary[key] = value;
		}

		/// <summary>Align padding to DWORD</summary>
		/// <param name="padding">Original padding</param>
		/// <returns>Aligned padding</returns>
		public static UInt32 AlignToInt(UInt32 padding)
		{
			/*UInt32 bytesToRead = padding % 4;
			if(bytesToRead > 0)
				padding += 4 - bytesToRead;
			return padding;*/
			return (UInt32)((padding + 3) & ~3);
		}

		/// <summary>Align padding to WORD</summary>
		/// <param name="padding">Original padding</param>
		/// <returns>Aligned padding</returns>
		public static UInt32 AlignToShort(UInt32 padding)
		{
			return (UInt32)((padding + 1) & ~1);
		}

		/// <summary>Remaining length in the reader</summary>
		/// <param name="reader">Reader where we need to calculate remaining length</param>
		/// <returns>Count of bytes stored in reader after current position</returns>
		public static Int32 Remaining(BinaryReader reader)
		{
			return (Int32)reader.BaseStream.Length - (Int32)reader.BaseStream.Position;
		}
	}
}