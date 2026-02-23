using System;
using System.IO;
using AlphaOmega.Debug;
using AlphaOmega.Debug.Dex;
using Moq;
using NUnit.Framework;

namespace ApkReader.Tests
{
	[TestFixture]
	public class DexFileTests
	{
		private Mock<IImageLoader> _mockLoader;

		[SetUp]
		public void SetUp()
		{
			_mockLoader = new Mock<IImageLoader>();
			_mockLoader.SetupProperty(x => x.Endianness, EndianHelper.Endian.Little);
		}

		[TearDown]
		public void TearDown()
		{
			_mockLoader = null;
		}

		private Byte[] CreateValidDexHeader()
		{
			// Create a minimal valid DEX header
			Byte[] header = new Byte[112]; // sizeof(header_item)
			
			// Magic bytes: "dex\n"
			header[0] = 0x64; // 'd'
			header[1] = 0x65; // 'e'
			header[2] = 0x78; // 'x'
			header[3] = 0x0a; // '\n'
			header[4] = 0x30; // '0'
			header[5] = 0x33; // '3'
			header[6] = 0x35; // '5'
			header[7] = 0x00; // '\0'

			// Checksum (offset 8-11) - will be calculated
			BitConverter.GetBytes((UInt32)0x12345678).CopyTo(header, 8);

			// SHA-1 signature (offset 12-31) - 20 bytes
			for (Int32 i = 12; i < 32; i++)
				header[i] = (Byte)(i % 256);

			// file_size (offset 32-35)
			BitConverter.GetBytes((UInt32)1024).CopyTo(header, 32);

			// header_size (offset 36-39) - should be 0x70 (112 bytes)
			BitConverter.GetBytes((UInt32)112).CopyTo(header, 36);

			// endian_tag (offset 40-43)
			BitConverter.GetBytes((UInt32)0x12345678).CopyTo(header, 40);

			// link_size and link_off (offset 44-51)
			BitConverter.GetBytes((UInt32)0).CopyTo(header, 44);
			BitConverter.GetBytes((UInt32)0).CopyTo(header, 48);

			// map_off (offset 52-55) - must be non-zero
			BitConverter.GetBytes((UInt32)512).CopyTo(header, 52);

			// string_ids_size and string_ids_off (offset 56-63)
			BitConverter.GetBytes((UInt32)0).CopyTo(header, 56);
			BitConverter.GetBytes((UInt32)0).CopyTo(header, 60);

			// type_ids_size and type_ids_off (offset 64-71)
			BitConverter.GetBytes((UInt32)0).CopyTo(header, 64);
			BitConverter.GetBytes((UInt32)0).CopyTo(header, 68);

			// proto_ids_size and proto_ids_off (offset 72-79)
			BitConverter.GetBytes((UInt32)0).CopyTo(header, 72);
			BitConverter.GetBytes((UInt32)0).CopyTo(header, 76);

			// field_ids_size and field_ids_off (offset 80-87)
			BitConverter.GetBytes((UInt32)0).CopyTo(header, 80);
			BitConverter.GetBytes((UInt32)0).CopyTo(header, 84);

			// method_ids_size and method_ids_off (offset 88-95)
			BitConverter.GetBytes((UInt32)0).CopyTo(header, 88);
			BitConverter.GetBytes((UInt32)0).CopyTo(header, 92);

			// class_defs_size and class_defs_off (offset 96-103)
			BitConverter.GetBytes((UInt32)0).CopyTo(header, 96);
			BitConverter.GetBytes((UInt32)0).CopyTo(header, 100);

			// data_size and data_off (offset 104-111)
			BitConverter.GetBytes((UInt32)512).CopyTo(header, 104);
			BitConverter.GetBytes((UInt32)112).CopyTo(header, 108);

			return header;
		}

		[Test]
		public void Constructor_WithNullLoader_ThrowsArgumentNullException()
		{
			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => new DexFile(null));
		}

		[Test]
		public void Constructor_WithInvalidHeader_ThrowsInvalidOperationException()
		{
			// Arrange
			Byte[] invalidHeader = new Byte[112];
			// Invalid magic bytes
			invalidHeader[0] = 0xFF;
			invalidHeader[1] = 0xFF;
			invalidHeader[2] = 0xFF;
			invalidHeader[3] = 0xFF;

			_mockLoader.Setup(x => x.PtrToStructure<DexApi.header_item>(0))
				.Returns(() =>
				{
					DexApi.header_item header = new DexApi.header_item
					{
						magic = new Byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x30, 0x33, 0x35, 0x00 }
					};
					return header;
				});

			// Act & Assert
			Assert.Throws<InvalidOperationException>(() => new DexFile(_mockLoader.Object));
		}

		[Test]
		public void Constructor_WithValidHeader_SetsPropertiesCorrectly()
		{
			// Arrange
			Byte[] validHeader = CreateValidDexHeader();
			
			_mockLoader.Setup(x => x.PtrToStructure<DexApi.header_item>(0))
				.Returns(() =>
				{
					DexApi.header_item header = new DexApi.header_item
					{
						magic = new Byte[] { 0x64, 0x65, 0x78, 0x0a, 0x30, 0x33, 0x35, 0x00 },
						checksum = 0x12345678,
						signature = new Byte[20],
						file_size = 1024,
						header_size = 112,
						endian_tag = DexApi.ENDIAN.ENDIAN,
						map_off = 512
					};
					return header;
				});

			_mockLoader.Setup(x => x.Length).Returns(1024);

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				// Assert
				Assert.IsNotNull(dexFile.Loader);
				Assert.IsNotNull(dexFile.Header);
				Assert.AreEqual(0x12345678U, dexFile.Header.checksum);
			}
		}

		[Test]
		public void Constructor_WithBigEndian_SetsEndiannessCorrectly()
		{
			// Arrange
			_mockLoader.SetupSequence(x => x.PtrToStructure<DexApi.header_item>(0))
				.Returns(() =>
				{
					DexApi.header_item header = new DexApi.header_item
					{
						magic = new Byte[] { 0x64, 0x65, 0x78, 0x0a, 0x30, 0x33, 0x35, 0x00 },
						checksum = 0x12345678,
						signature = new Byte[20],
						file_size = 1024,
						header_size = 112,
						endian_tag = DexApi.ENDIAN.REVERSE_ENDIAN,
						map_off = 512
					};
					return header;
				})
				.Returns(() =>
				{
					DexApi.header_item header = new DexApi.header_item
					{
						magic = new Byte[] { 0x64, 0x65, 0x78, 0x0a, 0x30, 0x33, 0x35, 0x00 },
						checksum = 0x78563412,
						signature = new Byte[20],
						file_size = 1024,
						header_size = 112,
						endian_tag = DexApi.ENDIAN.REVERSE_ENDIAN,
						map_off = 512
					};
					return header;
				});

			_mockLoader.Setup(x => x.Length).Returns(1024);

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				// Assert
				_mockLoader.VerifySet(x => x.Endianness = EndianHelper.Endian.Big, Times.Once);
			}
		}

		[Test]
		public void PtrToStructure_CallsLoaderMethod()
		{
			// Arrange
			SetupValidDexFile();
			
			DexApi.header_item expectedHeader = new DexApi.header_item();
			_mockLoader.Setup(x => x.PtrToStructure<DexApi.header_item>(100))
				.Returns(expectedHeader);

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				DexApi.header_item result = dexFile.PtrToStructure<DexApi.header_item>(100);

				// Assert
				_mockLoader.Verify(x => x.PtrToStructure<DexApi.header_item>(100), Times.Once);
			}
		}

		[Test]
		public void PtrToStringAnsi_CallsLoaderMethod()
		{
			// Arrange
			SetupValidDexFile();
			
			_mockLoader.Setup(x => x.PtrToStringAnsi(100))
				.Returns("TestString");

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				String result = dexFile.PtrToStringAnsi(100);

				// Assert
				Assert.AreEqual("TestString", result);
				_mockLoader.Verify(x => x.PtrToStringAnsi(100), Times.Once);
			}
		}

		[Test]
		public void ReadULeb128_SingleByte_ReturnsCorrectValue()
		{
			// Arrange
			SetupValidDexFile();
			
			// Single byte value: 0x05 (no continuation bit)
			_mockLoader.Setup(x => x.ReadBytes(100, 1))
				.Returns(new Byte[] { 0x05 });

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				UInt32 offset = 100;
				Int32 result = dexFile.ReadULeb128(ref offset);

				// Assert
				Assert.AreEqual(5, result);
				Assert.AreEqual(101U, offset);
			}
		}

		[Test]
		public void ReadULeb128_MultiByte_ReturnsCorrectValue()
		{
			// Arrange
			SetupValidDexFile();
			
			// Multi-byte value: 0x80, 0x01 = 128
			_mockLoader.SetupSequence(x => x.ReadBytes(It.IsAny<UInt32>(), 1))
				.Returns(new Byte[] { 0x80 })
				.Returns(new Byte[] { 0x01 });

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				UInt32 offset = 100;
				Int32 result = dexFile.ReadULeb128(ref offset);

				// Assert
				Assert.AreEqual(128, result);
				Assert.AreEqual(102U, offset);
			}
		}

		[Test]
		public void ReadULeb128_InvalidSequence_ThrowsException()
		{
			// Arrange
			SetupValidDexFile();
			
			// Invalid sequence: 5 bytes with continuation bit set
			_mockLoader.Setup(x => x.ReadBytes(It.IsAny<UInt32>(), 1))
				.Returns(new Byte[] { 0x80 });

			// Act & Assert
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				UInt32 offset = 100;
				Assert.Throws<InvalidOperationException>(() => dexFile.ReadULeb128(ref offset));
			}
		}

		[Test]
		public void ReadSLeb128_PositiveValue_ReturnsCorrectValue()
		{
			// Arrange
			SetupValidDexFile();
			
			// Positive single byte value
			_mockLoader.Setup(x => x.ReadBytes(100, 1))
				.Returns(new Byte[] { 0x05 });

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				UInt32 offset = 100;
				Int32 result = dexFile.ReadSLeb128(ref offset);

				// Assert
				Assert.AreEqual(5, result);
				Assert.AreEqual(101U, offset);
			}
		}

		[Test]
		public void ReadSLeb128_NegativeValue_ReturnsCorrectValue()
		{
			// Arrange
			SetupValidDexFile();
			
			// Negative value: 0x7F = -1
			_mockLoader.Setup(x => x.ReadBytes(100, 1))
				.Returns(new Byte[] { 0x7F });

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				UInt32 offset = 100;
				Int32 result = dexFile.ReadSLeb128(ref offset);

				// Assert
				Assert.AreEqual(-1, result);
				Assert.AreEqual(101U, offset);
			}
		}

		[Test]
		public void ReadMUtf8_AsciiString_ReturnsCorrectValue()
		{
			// Arrange
			SetupValidDexFile();
			
			// String length: 5 (encoded as ULEB128)
			_mockLoader.SetupSequence(x => x.ReadBytes(It.IsAny<UInt32>(), It.IsAny<UInt32>()))
				.Returns(new Byte[] { 0x05 }) // ULEB128 length
				.Returns(new Byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }); // "Hello\0" with padding

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				UInt32 offset = 100;
				String result = dexFile.ReadMUtf8(out Int32 utf16Size, ref offset);

				// Assert
				Assert.AreEqual("Hello", result);
				Assert.AreEqual(5, utf16Size);
			}
		}

		[Test]
		public void ReadMUtf8_NullCharacter_HandlesCorrectly()
		{
			// Arrange
			SetupValidDexFile();
			
			// MUTF-8 null character encoding: 0xC0 0x80
			_mockLoader.SetupSequence(x => x.ReadBytes(It.IsAny<UInt32>(), It.IsAny<UInt32>()))
				.Returns(new Byte[] { 0x01 }) // ULEB128 length = 1
				.Returns(new Byte[] { 0xC0, 0x80, 0x00, 0x00 }); // MUTF-8 null + terminator + padding

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				UInt32 offset = 100;
				String result = dexFile.ReadMUtf8(out Int32 utf16Size, ref offset);

				// Assert
				Assert.AreEqual("\0", result);
				Assert.AreEqual(1, utf16Size);
			}
		}

		[Test]
		public void ReadMUtf8_TwoByteSequence_ReturnsCorrectValue()
		{
			// Arrange
			SetupValidDexFile();
			
			// Two-byte sequence: 0xC2 0xA9 = © (copyright symbol)
			_mockLoader.SetupSequence(x => x.ReadBytes(It.IsAny<UInt32>(), It.IsAny<UInt32>()))
				.Returns(new Byte[] { 0x01 }) // ULEB128 length = 1
				.Returns(new Byte[] { 0xC2, 0xA9, 0x00, 0x00 }); // © + terminator + padding

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				UInt32 offset = 100;
				String result = dexFile.ReadMUtf8(out Int32 utf16Size, ref offset);

				// Assert
				Assert.AreEqual("©", result);
				Assert.AreEqual(1, utf16Size);
			}
		}

		[Test]
		public void ReadMUtf8_ThreeByteSequence_ReturnsCorrectValue()
		{
			// Arrange
			SetupValidDexFile();
			
			// Three-byte sequence: 0xE2 0x82 0xAC = € (euro symbol)
			_mockLoader.SetupSequence(x => x.ReadBytes(It.IsAny<UInt32>(), It.IsAny<UInt32>()))
				.Returns(new Byte[] { 0x01 }) // ULEB128 length = 1
				.Returns(new Byte[] { 0xE2, 0x82, 0xAC, 0x00 }); // € + terminator

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				UInt32 offset = 100;
				String result = dexFile.ReadMUtf8(out Int32 utf16Size, ref offset);

				// Assert
				Assert.AreEqual("€", result);
				Assert.AreEqual(1, utf16Size);
			}
		}

		[Test]
		public void ReadMUtf8_MissingNullTerminator_ThrowsException()
		{
			// Arrange
			SetupValidDexFile();
			
			_mockLoader.SetupSequence(x => x.ReadBytes(It.IsAny<UInt32>(), It.IsAny<UInt32>()))
				.Returns(new Byte[] { 0x05 }) // ULEB128 length
				.Returns(new Byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x48, 0x48, 0x48, 0x48, 0x48, 0x48, 0x48, 0x48, 0x48, 0x48, 0x48 }); // No null terminator

			// Act & Assert
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				UInt32 offset = 100;
				Assert.Throws<InvalidOperationException>(() => dexFile.ReadMUtf8(out Int32 utf16Size, ref offset));
			}
		}

		[Test]
		public void ReadMUtf8_FourByteSequence_ThrowsException()
		{
			// Arrange
			SetupValidDexFile();
			
			// Invalid 4-byte sequence
			_mockLoader.SetupSequence(x => x.ReadBytes(It.IsAny<UInt32>(), It.IsAny<UInt32>()))
				.Returns(new Byte[] { 0x01 }) // ULEB128 length
				.Returns(new Byte[] { 0xF0, 0x9F, 0x98, 0x80, 0x00 }); // 4-byte + terminator

			// Act & Assert
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				UInt32 offset = 100;
				Assert.Throws<InvalidOperationException>(() => dexFile.ReadMUtf8(out Int32 utf16Size, ref offset));
			}
		}

		[Test]
		public void GetMapItem_ExistingType_ReturnsCorrectItem()
		{
			// Arrange
			SetupValidDexFileWithMapList();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				DexApi.map_item? result = dexFile.GetMapItem(DexApi.TYPE.STRING_ID_ITEM);

				// Assert
				Assert.IsNotNull(result);
				Assert.AreEqual(DexApi.TYPE.STRING_ID_ITEM, result.Value.type);
			}
		}

		[Test]
		public void GetMapItem_NonExistingType_ReturnsNull()
		{
			// Arrange
			SetupValidDexFileWithMapList();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				DexApi.map_item? result = dexFile.GetMapItem((DexApi.TYPE)0xFFFF);

				// Assert
				Assert.IsNull(result);
			}
		}

		[Test]
		public void Dispose_DisposesLoader()
		{
			// Arrange
			SetupValidDexFile();
			DexFile dexFile = new DexFile(_mockLoader.Object);

			// Act
			dexFile.Dispose();

			// Assert
			_mockLoader.Verify(x => x.Dispose(), Times.Once);
		}

		[Test]
		public void Dispose_CalledMultipleTimes_DisposesOnlyOnce()
		{
			// Arrange
			SetupValidDexFile();
			DexFile dexFile = new DexFile(_mockLoader.Object);

			// Act
			dexFile.Dispose();
			dexFile.Dispose();

			// Assert
			_mockLoader.Verify(x => x.Dispose(), Times.Once);
		}

		[Test]
		public void IsChecksumValid_WithValidChecksum_ReturnsTrue()
		{
			// Arrange
			Byte[] validHeader = CreateValidDexHeader();
			Byte[] fullFile = new Byte[1024];
			Array.Copy(validHeader, 0, fullFile, 0, validHeader.Length);

			// Calculate valid checksum
			const UInt32 MOD_ADLER = 65521;
			UInt32 a = 1, b = 0;
			for (Int32 i = 12; i < fullFile.Length; i++)
			{
				a = (a + fullFile[i]) % MOD_ADLER;
				b = (b + a) % MOD_ADLER;
			}
			UInt32 validChecksum = (b << 16) | a;
			BitConverter.GetBytes(validChecksum).CopyTo(fullFile, 8);

			_mockLoader.Setup(x => x.PtrToStructure<DexApi.header_item>(0))
				.Returns(() =>
				{
					DexApi.header_item header = new DexApi.header_item
					{
						magic = new Byte[] { 0x64, 0x65, 0x78, 0x0a, 0x30, 0x33, 0x35, 0x00 },
						checksum = validChecksum,
						signature = new Byte[20],
						file_size = 1024,
						header_size = 112,
						endian_tag = DexApi.ENDIAN.ENDIAN,
						map_off = 512
					};
					return header;
				});

			_mockLoader.Setup(x => x.Length).Returns(1024);
			_mockLoader.Setup(x => x.ReadBytes(0, 1024)).Returns(fullFile);

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				Boolean result = dexFile.IsChecksumValid;

				// Assert
				Assert.IsTrue(result);
			}
		}

		[Test]
		public void IsChecksumValid_WithInvalidChecksum_ReturnsFalse()
		{
			// Arrange
			Byte[] validHeader = CreateValidDexHeader();
			Byte[] fullFile = new Byte[1024];
			Array.Copy(validHeader, 0, fullFile, 0, validHeader.Length);

			// Set an invalid checksum
			UInt32 invalidChecksum = 0xDEADBEEF;
			BitConverter.GetBytes(invalidChecksum).CopyTo(fullFile, 8);

			_mockLoader.Setup(x => x.PtrToStructure<DexApi.header_item>(0))
				.Returns(() =>
				{
					DexApi.header_item header = new DexApi.header_item
					{
						magic = new Byte[] { 0x64, 0x65, 0x78, 0x0a, 0x30, 0x33, 0x35, 0x00 },
						checksum = invalidChecksum,
						signature = new Byte[20],
						file_size = 1024,
						header_size = 112,
						endian_tag = DexApi.ENDIAN.ENDIAN,
						map_off = 512
					};
					return header;
				});

			_mockLoader.Setup(x => x.Length).Returns(1024);
			_mockLoader.Setup(x => x.ReadBytes(0, 1024)).Returns(fullFile);

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				Boolean result = dexFile.IsChecksumValid;

				// Assert
				Assert.IsFalse(result);
			}
		}

		[Test]
		public void STRING_ID_ITEM_ReturnsBaseTable()
		{
			// Arrange
			SetupValidDexFile();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				var result = dexFile.STRING_ID_ITEM;

				// Assert
				Assert.IsNotNull(result);
			}
		}

		[Test]
		public void STRING_DATA_ITEM_ReturnsBaseTable()
		{
			// Arrange
			SetupValidDexFile();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				var result = dexFile.STRING_DATA_ITEM;

				// Assert
				Assert.IsNotNull(result);
			}
		}

		[Test]
		public void CODE_ITEM_ReturnsBaseTable()
		{
			// Arrange
			SetupValidDexFile();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				var result = dexFile.CODE_ITEM;

				// Assert
				Assert.IsNotNull(result);
			}
		}

		[Test]
		public void TYPE_ID_ITEM_ReturnsBaseTable()
		{
			// Arrange
			SetupValidDexFile();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				var result = dexFile.TYPE_ID_ITEM;

				// Assert
				Assert.IsNotNull(result);
			}
		}

		[Test]
		public void TYPE_LIST_WithNullTable_ReturnsNull()
		{
			// Arrange
			SetupValidDexFileWithEmptyMapList();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				var result = dexFile.TYPE_LIST;

				// Assert
				Assert.IsNull(result);
			}
		}

		[Test]
		public void PROTO_ID_ITEM_ReturnsBaseTable()
		{
			// Arrange
			SetupValidDexFile();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				var result = dexFile.PROTO_ID_ITEM;

				// Assert
				Assert.IsNotNull(result);
			}
		}

		[Test]
		public void FIELD_ID_ITEM_ReturnsBaseTable()
		{
			// Arrange
			SetupValidDexFile();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				var result = dexFile.FIELD_ID_ITEM;

				// Assert
				Assert.IsNotNull(result);
			}
		}

		[Test]
		public void METHOD_ID_ITEM_ReturnsBaseTable()
		{
			// Arrange
			SetupValidDexFile();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				var result = dexFile.METHOD_ID_ITEM;

				// Assert
				Assert.IsNotNull(result);
			}
		}

		[Test]
		public void CLASS_DEF_ITEM_ReturnsBaseTable()
		{
			// Arrange
			SetupValidDexFile();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				var result = dexFile.CLASS_DEF_ITEM;

				// Assert
				Assert.IsNotNull(result);
			}
		}

		[Test]
		public void ANNOTATIONS_DIRECTORY_ITEM_WithNullTable_ReturnsNull()
		{
			// Arrange
			SetupValidDexFileWithEmptyMapList();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				var result = dexFile.ANNOTATIONS_DIRECTORY_ITEM;

				// Assert
				Assert.IsNull(result);
			}
		}

		[Test]
		public void ANNOTATION_SET_REF_LIST_WithNullTable_ReturnsNull()
		{
			// Arrange
			SetupValidDexFileWithEmptyMapList();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				var result = dexFile.ANNOTATION_SET_REF_LIST;

				// Assert
				Assert.IsNull(result);
			}
		}

		[Test]
		public void map_list_LoadsMapItemsCorrectly()
		{
			// Arrange
			SetupValidDexFileWithMapList();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				DexApi.map_item[] result = dexFile.map_list;

				// Assert
				Assert.IsNotNull(result);
				Assert.AreEqual(1, result.Length);
				Assert.AreEqual(DexApi.TYPE.STRING_ID_ITEM, result[0].type);
			}
		}

		[Test]
		public void GetSectionTable_CachesResults()
		{
			// Arrange
			SetupValidDexFileWithMapList();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				Table result1 = dexFile.GetSectionTable(TableType.STRING_ID_ITEM);
				Table result2 = dexFile.GetSectionTable(TableType.STRING_ID_ITEM);

				// Assert - Should return the same cached instance
				Assert.AreSame(result1, result2);
			}
		}

		[Test]
		public void GetSectionTable_WithNonExistingSection_ReturnsNull()
		{
			// Arrange
			SetupValidDexFileWithEmptyMapList();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				Table result = dexFile.GetSectionTable(TableType.TYPE_LIST);

				// Assert
				Assert.IsNull(result);
			}
		}

		[Test]
		public void GetSectionTables_ReturnsAllTables()
		{
			// Arrange
			SetupValidDexFileWithMapList();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				Int32 count = 0;
				foreach (Table table in dexFile.GetSectionTables())
				{
					count++;
				}

				// Assert
				Assert.Greater(count, 0);
			}
		}

		[Test]
		public void ReadSectionTable_WithValidType_ReturnsTable()
		{
			// Arrange
			SetupValidDexFileWithMapList();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				Table result = dexFile.ReadSectionTable(TableType.STRING_ID_ITEM);

				// Assert
				Assert.IsNotNull(result);
			}
		}

		[Test]
		public void Loader_ReturnsSetLoader()
		{
			// Arrange
			SetupValidDexFile();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				// Assert
				Assert.AreSame(_mockLoader.Object, dexFile.Loader);
			}
		}

		[Test]
		public void Header_ReturnsValidHeader()
		{
			// Arrange
			SetupValidDexFile();

			// Act
			using (DexFile dexFile = new DexFile(_mockLoader.Object))
			{
				// Assert
				Assert.IsTrue(dexFile.Header.IsValid);
				Assert.AreEqual(0x12345678U, dexFile.Header.checksum);
			}
		}

		private void SetupValidDexFile()
		{
			_mockLoader.Setup(x => x.PtrToStructure<DexApi.header_item>(0))
				.Returns(() =>
				{
					DexApi.header_item header = new DexApi.header_item
					{
						magic = new Byte[] { 0x64, 0x65, 0x78, 0x0a, 0x30, 0x33, 0x35, 0x00 },
						checksum = 0x12345678,
						signature = new Byte[20],
						file_size = 1024,
						header_size = 112,
						endian_tag = DexApi.ENDIAN.ENDIAN,
						map_off = 512
					};
					return header;
				});

			_mockLoader.Setup(x => x.Length).Returns(1024);
		}

		private void SetupValidDexFileWithMapList()
		{
			SetupValidDexFile();

			// Setup map_list
			_mockLoader.Setup(x => x.PtrToStructure<UInt32>(512))
				.Returns(1U); // 1 map item

			_mockLoader.Setup(x => x.PtrToStructure<DexApi.map_item>(516))
				.Returns(new DexApi.map_item
				{
					type = DexApi.TYPE.STRING_ID_ITEM,
					size = 10,
					offset = 100
				});
		}

		private void SetupValidDexFileWithEmptyMapList()
		{
			SetupValidDexFile();

			// Setup empty map_list
			_mockLoader.Setup(x => x.PtrToStructure<UInt32>(512))
				.Returns(0U); // 0 map items
		}
	}
}
