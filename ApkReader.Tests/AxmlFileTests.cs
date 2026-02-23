using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlphaOmega.Debug;
using AlphaOmega.Debug.Axml;
using Moq;
using NUnit.Framework;

namespace ApkReader.Tests
{
	[TestFixture]
	public class AxmlFileTests
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

		private AxmlApi.AxmlFileHeader CreateValidAxmlHeader(UInt32 stringCount = 2, Boolean isUtf8 = false)
		{
			return new AxmlApi.AxmlFileHeader
			{
				MagicNumber = 0x00080003,
				FileSize = 1024,
				StringPool = new AxmlApi.StringPoolHeader
				{
					ChunkType = 0x001C0001,
					ChunkSize = 512,
					stringCount = stringCount,
					StyleCount = 0,
					Flags = isUtf8 ? 0x00000100U : 0x00000000U,
					stringsStart = 100,
					stylesStart = 0
				}
			};
		}

		[Test]
		public void Constructor_WithNullLoader_ThrowsArgumentNullException()
		{
			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => new AxmlFile(null));
		}

		[Test]
		public void Constructor_WithValidLoader_SetsPropertiesCorrectly()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader();
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			// Act
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				// Assert
				Assert.IsNotNull(axmlFile.Header);
				Assert.AreEqual(0x00080003U, axmlFile.Header.MagicNumber);
				Assert.IsTrue(axmlFile.Header.IsValid);
			}
		}

		[Test]
		public void Constructor_SetsEndiannessToLittle()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader();
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			// Act
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				// Assert
				_mockLoader.VerifySet(x => x.Endianness = EndianHelper.Endian.Little, Times.Once);
			}
		}

		[Test]
		public void Header_WithInvalidMagicNumber_ReturnsInvalidHeader()
		{
			// Arrange
			AxmlApi.AxmlFileHeader invalidHeader = CreateValidAxmlHeader();
			invalidHeader.MagicNumber = 0xFFFFFFFF;

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(invalidHeader);

			// Act
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				// Assert
				Assert.IsFalse(axmlFile.Header.IsValid);
			}
		}

		[Test]
		public void Strings_WithInvalidHeader_ThrowsInvalidOperationException()
		{
			// Arrange
			AxmlApi.AxmlFileHeader invalidHeader = CreateValidAxmlHeader();
			invalidHeader.MagicNumber = 0xFFFFFFFF;

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(invalidHeader);

			// Act & Assert
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				Assert.Throws<InvalidOperationException>(() => { var _ = axmlFile.Strings; });
			}
		}

		[Test]
		public void Strings_WithUtf16Encoding_ReadsStringsCorrectly()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader(2, false); // UTF-16
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			// Setup string offsets
			_mockLoader.SetupSequence(x => x.PtrToStructure<UInt32>(It.IsAny<UInt32>()))
				.Returns(0U)  // First string offset
				.Returns(12U); // Second string offset

			// Setup string lengths and data
			// First string "Hello" (5 chars)
			_mockLoader.SetupSequence(x => x.PtrToStructure<Int16>(It.IsAny<UInt32>()))
				.Returns(5)   // Length of first string
				.Returns(4);  // Length of second string

			// String data for "Hello" in UTF-16
			Byte[] helloUtf16 = Encoding.Unicode.GetBytes("Hello");
			Byte[] testUtf16 = Encoding.Unicode.GetBytes("Test");

			var readBytesSetup = _mockLoader.SetupSequence(x => x.ReadBytes(It.IsAny<UInt32>(), It.IsAny<UInt32>()));
			readBytesSetup.Returns(helloUtf16);
			readBytesSetup.Returns(testUtf16);

			// Act
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				String[] strings = axmlFile.Strings;

				// Assert
				Assert.IsNotNull(strings);
				Assert.AreEqual(2, strings.Length);
				Assert.AreEqual("Hello", strings[0]);
				Assert.AreEqual("Test", strings[1]);
			}
		}

		[Test]
		public void Strings_WithUtf8Encoding_ReadsStringsCorrectly()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader(2, true); // UTF-8
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			// Setup string offsets
			_mockLoader.SetupSequence(x => x.PtrToStructure<UInt32>(It.IsAny<UInt32>()))
				.Returns(0U)  // First string offset
				.Returns(8U); // Second string offset

			// Setup ReadLen8 calls (char length + byte length for each string)
			_mockLoader.SetupSequence(x => x.PtrToStructure<Byte>(It.IsAny<UInt32>()))
				.Returns(5)   // First string char length
				.Returns(5)   // First string byte length
				.Returns(4)   // Second string char length
				.Returns(4);  // Second string byte length

			// String data in UTF-8
			Byte[] helloUtf8 = Encoding.UTF8.GetBytes("Hello");
			Byte[] testUtf8 = Encoding.UTF8.GetBytes("Test");

			var readBytesSetup = _mockLoader.SetupSequence(x => x.ReadBytes(It.IsAny<UInt32>(), It.IsAny<UInt32>()));
			readBytesSetup.Returns(helloUtf8);
			readBytesSetup.Returns(testUtf8);

			// Act
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				String[] strings = axmlFile.Strings;

				// Assert
				Assert.IsNotNull(strings);
				Assert.AreEqual(2, strings.Length);
				Assert.AreEqual("Hello", strings[0]);
				Assert.AreEqual("Test", strings[1]);
			}
		}

		[Test]
		public void Strings_CalledMultipleTimes_ReturnsCachedResult()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader(1, false);
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			_mockLoader.Setup(x => x.PtrToStructure<UInt32>(It.IsAny<UInt32>()))
				.Returns(0U);

			_mockLoader.Setup(x => x.PtrToStructure<Int16>(It.IsAny<UInt32>()))
				.Returns(5);

			Byte[] testData = Encoding.Unicode.GetBytes("Hello");
			_mockLoader.Setup(x => x.ReadBytes(It.IsAny<UInt32>(), It.IsAny<UInt32>()))
				.Returns(testData);

			// Act
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				String[] strings1 = axmlFile.Strings;
				String[] strings2 = axmlFile.Strings;

				// Assert
				Assert.AreSame(strings1, strings2);
			}
		}

		[Test]
		public void ReadXmlChunks_WithInvalidHeader_ThrowsInvalidOperationException()
		{
			// Arrange
			AxmlApi.AxmlFileHeader invalidHeader = CreateValidAxmlHeader();
			invalidHeader.MagicNumber = 0xFFFFFFFF;

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(invalidHeader);

			// Act & Assert
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				Assert.Throws<InvalidOperationException>(() => axmlFile.ReadXmlChunks().ToList());
			}
		}

		[Test]
		public void ReadXmlChunks_WithStartNamespace_ReturnsDocumentChunk()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader();
			header.FileSize = 600;
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			// Setup chunk reading
			AxmlApi.Chunk chunk = new AxmlApi.Chunk
			{
				TagType = AxmlApi.ChunkType.RES_XML_START_NAMESPACE_TYPE,
				HeaderSize = 16,
				ChunkSize = 600
			};

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.Chunk>(It.IsAny<UInt32>()))
				.Returns(chunk);

			AxmlApi.Document document = new AxmlApi.Document
			{
				@namespace = 0,
				name = 0
			};

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.Document>(It.IsAny<UInt32>()))
				.Returns(document);

			// Act
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				List<AxmlChunk> chunks = axmlFile.ReadXmlChunks().ToList();

				// Assert
				Assert.AreEqual(1, chunks.Count);
				Assert.AreEqual(AxmlApi.ChunkType.RES_XML_START_NAMESPACE_TYPE, chunks[0].Chunk.TagType);
				Assert.IsNotNull(chunks[0].Document);
			}
		}

		[Test]
		public void ReadXmlChunks_WithStartTag_ReturnsStartTagChunkWithAttributes()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader();
			header.FileSize = 600;
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			// Setup chunk reading
			AxmlApi.Chunk chunk = new AxmlApi.Chunk
			{
				TagType = AxmlApi.ChunkType.RES_XML_START_ELEMENT_TYPE,
				HeaderSize = 16,
				ChunkSize = 600
			};

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.Chunk>(It.IsAny<UInt32>()))
				.Returns(chunk);

			AxmlApi.StartTag startTag = new AxmlApi.StartTag
			{
				tagName = 0,
				attributeCount = 2,
				flags = 0
			};

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.StartTag>(It.IsAny<UInt32>()))
				.Returns(startTag);

			AxmlApi.Attribute attribute1 = new AxmlApi.Attribute
			{
				NamespaceIndex = 0,
				NameIndex = 1,
				RawValueIndex = 2,
				Data = 0
			};

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.Attribute>(It.IsAny<UInt32>()))
				.Returns(attribute1);

			// Act
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				List<AxmlChunk> chunks = axmlFile.ReadXmlChunks().ToList();

				// Assert
				Assert.AreEqual(1, chunks.Count);
				Assert.AreEqual(AxmlApi.ChunkType.RES_XML_START_ELEMENT_TYPE, chunks[0].Chunk.TagType);
				Assert.IsNotNull(chunks[0].StartTag);
				Assert.AreEqual(2, chunks[0].Attributes.Length);
			}
		}

		[Test]
		public void ReadXmlChunks_WithEndTag_ReturnsEndTagChunk()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader();
			header.FileSize = 600;
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			// Setup chunk reading for EndTag
			AxmlApi.Chunk chunk = new AxmlApi.Chunk
			{
				TagType = AxmlApi.ChunkType.RES_XML_END_ELEMENT_TYPE,
				HeaderSize = 16,
				ChunkSize = 600
			};

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.Chunk>(It.IsAny<UInt32>()))
				.Returns(chunk);

			AxmlApi.EndTag endTag = new AxmlApi.EndTag
			{
				tagName = 0
			};

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.EndTag>(It.IsAny<UInt32>()))
				.Returns(endTag);

			// Act
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				List<AxmlChunk> chunks = axmlFile.ReadXmlChunks().ToList();

				// Assert
				Assert.AreEqual(1, chunks.Count);
				Assert.AreEqual(AxmlApi.ChunkType.RES_XML_END_ELEMENT_TYPE, chunks[0].Chunk.TagType);
				Assert.IsNotNull(chunks[0].EndTag);
			}
		}

		[Test]
		public void ReadXmlChunks_WithTextTag_ReturnsTextChunk()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader();
			header.FileSize = 600;
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			// Setup chunk reading for Text
			AxmlApi.Chunk chunk = new AxmlApi.Chunk
			{
				TagType = AxmlApi.ChunkType.RES_XML_CDATA_TYPE,
				HeaderSize = 16,
				ChunkSize = 600
			};

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.Chunk>(It.IsAny<UInt32>()))
				.Returns(chunk);

			AxmlApi.Text text = new AxmlApi.Text
			{
				tagName = 0,
				unk1 = 0,
				unk2 = 0
			};

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.Text>(It.IsAny<UInt32>()))
				.Returns(text);

			// Act
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				List<AxmlChunk> chunks = axmlFile.ReadXmlChunks().ToList();

				// Assert
				Assert.AreEqual(1, chunks.Count);
				Assert.AreEqual(AxmlApi.ChunkType.RES_XML_CDATA_TYPE, chunks[0].Chunk.TagType);
				Assert.IsNotNull(chunks[0].Text);
			}
		}

		[Test]
		public void ReadXmlChunks_WithUnsupportedChunkType_ThrowsNotImplementedException()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader();
			header.FileSize = 600;
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			// Setup chunk reading with unsupported type
			AxmlApi.Chunk chunk = new AxmlApi.Chunk
			{
				TagType = AxmlApi.ChunkType.Comment, // Unsupported type
				HeaderSize = 16,
				ChunkSize = 600
			};

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.Chunk>(It.IsAny<UInt32>()))
				.Returns(chunk);

			// Act & Assert
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				Assert.Throws<NotImplementedException>(() => axmlFile.ReadXmlChunks().ToList());
			}
		}

		[Test]
		public void ReadXmlChunks_WithResourceMapType_SkipsChunk()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader();
			header.FileSize = 600;
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			// Setup resource map chunk (should be skipped without yielding)
			AxmlApi.Chunk chunk = new AxmlApi.Chunk
			{
				TagType = AxmlApi.ChunkType.RES_XML_RESOURCE_MAP_TYPE,
				HeaderSize = 8,
				ChunkSize = 600
			};

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.Chunk>(It.IsAny<UInt32>()))
				.Returns(chunk);

			// Act
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				List<AxmlChunk> chunks = axmlFile.ReadXmlChunks().ToList();

				// Assert - Resource map chunks are not yielded
				Assert.AreEqual(0, chunks.Count);
			}
		}

		[Test]
		public void RootNode_WithInvalidHeader_ThrowsInvalidOperationException()
		{
			// Arrange
			AxmlApi.AxmlFileHeader invalidHeader = CreateValidAxmlHeader();
			invalidHeader.MagicNumber = 0xFFFFFFFF;

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(invalidHeader);

			// Act & Assert
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				Assert.Throws<InvalidOperationException>(() => { var _ = axmlFile.RootNode; });
			}
		}

		[Test]
		public void RootNode_WithValidXml_ReturnsXmlNodeTree()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader(3, false);
			header.FileSize = 600;
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			// Setup strings
			SetupStringReading(new[] { "manifest", "package", "com.example" });

			// Setup chunks: StartTag with attribute, then EndTag
			var chunkSequence = _mockLoader.SetupSequence(x => x.PtrToStructure<AxmlApi.Chunk>(It.IsAny<UInt32>()));
			
			chunkSequence.Returns(new AxmlApi.Chunk 
			{ 
				TagType = AxmlApi.ChunkType.RES_XML_START_ELEMENT_TYPE,
				HeaderSize = 16,
				ChunkSize = 100
			});
			chunkSequence.Returns(new AxmlApi.Chunk 
			{ 
				TagType = AxmlApi.ChunkType.RES_XML_END_ELEMENT_TYPE,
				HeaderSize = 16,
				ChunkSize = 500
			});

			AxmlApi.StartTag startTag = new AxmlApi.StartTag
			{
				tagName = 0, // "manifest"
				attributeCount = 1,
				flags = 0
			};

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.StartTag>(It.IsAny<UInt32>()))
				.Returns(startTag);

			AxmlApi.Attribute attribute = new AxmlApi.Attribute
			{
				NamespaceIndex = -1,
				NameIndex = 1, // "package"
				RawValueIndex = 2, // "com.example"
				ValueType = AxmlApi.DataType.STRING,
				Data = 2
			};

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.Attribute>(It.IsAny<UInt32>()))
				.Returns(attribute);

			AxmlApi.EndTag endTag = new AxmlApi.EndTag
			{
				tagName = 0
			};

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.EndTag>(It.IsAny<UInt32>()))
				.Returns(endTag);

			// Act
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				XmlNode rootNode = axmlFile.RootNode;

				// Assert
				Assert.IsNotNull(rootNode);
				Assert.AreEqual("manifest", rootNode.NodeName);
				Assert.IsNotNull(rootNode.Attributes);
				Assert.IsTrue(rootNode.Attributes.ContainsKey("package"));
			}
		}

		[Test]
		public void RootNode_CalledMultipleTimes_ReturnsCachedResult()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader(1, false);
			header.FileSize = 600;
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			SetupStringReading(new[] { "root" });

			var chunkSequence = _mockLoader.SetupSequence(x => x.PtrToStructure<AxmlApi.Chunk>(It.IsAny<UInt32>()));
			chunkSequence.Returns(new AxmlApi.Chunk 
			{ 
				TagType = AxmlApi.ChunkType.RES_XML_START_ELEMENT_TYPE,
				HeaderSize = 16,
				ChunkSize = 100
			});
			chunkSequence.Returns(new AxmlApi.Chunk 
			{ 
				TagType = AxmlApi.ChunkType.RES_XML_END_ELEMENT_TYPE,
				HeaderSize = 16,
				ChunkSize = 500
			});

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.StartTag>(It.IsAny<UInt32>()))
				.Returns(new AxmlApi.StartTag { tagName = 0, attributeCount = 0 });

			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.EndTag>(It.IsAny<UInt32>()))
				.Returns(new AxmlApi.EndTag { tagName = 0 });

			// Act
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				XmlNode rootNode1 = axmlFile.RootNode;
				XmlNode rootNode2 = axmlFile.RootNode;

				// Assert
				Assert.AreSame(rootNode1, rootNode2);
			}
		}

		[Test]
		public void Dispose_DisposesLoader()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader();
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			AxmlFile axmlFile = new AxmlFile(_mockLoader.Object);

			// Act
			axmlFile.Dispose();

			// Assert
			_mockLoader.Verify(x => x.Dispose(), Times.Once);
		}

		[Test]
		public void Dispose_CalledMultipleTimes_DisposesOnlyOnce()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader();
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			AxmlFile axmlFile = new AxmlFile(_mockLoader.Object);

			// Act
			axmlFile.Dispose();
			axmlFile.Dispose();

			// Assert
			_mockLoader.Verify(x => x.Dispose(), Times.Once);
		}

		[Test]
		public void ReadLen8_WithSingleByte_ReturnsCorrectLength()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader(1, true);
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			// Single byte length (no high bit set)
			_mockLoader.SetupSequence(x => x.PtrToStructure<Byte>(It.IsAny<UInt32>()))
				.Returns(0x05); // Length = 5

			_mockLoader.Setup(x => x.PtrToStructure<UInt32>(It.IsAny<UInt32>()))
				.Returns(0U);

			Byte[] testData = Encoding.UTF8.GetBytes("Hello");
			_mockLoader.Setup(x => x.ReadBytes(It.IsAny<UInt32>(), It.IsAny<UInt32>()))
				.Returns(testData);

			// Act
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				String[] strings = axmlFile.Strings;

				// Assert
				Assert.AreEqual("Hello", strings[0]);
			}
		}

		[Test]
		public void ReadLen8_WithTwoBytes_ReturnsCorrectLength()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader(1, true);
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			// Two byte length (high bit set in first byte)
			_mockLoader.SetupSequence(x => x.PtrToStructure<Byte>(It.IsAny<UInt32>()))
				.Returns(0x80) // High bit set, char length high byte
				.Returns(0x01) // Char length low byte = 1
				.Returns(0x81) // High bit set, byte length high byte
				.Returns(0x00); // Byte length low byte = 128

			_mockLoader.Setup(x => x.PtrToStructure<UInt32>(It.IsAny<UInt32>()))
				.Returns(0U);

			Byte[] testData = new Byte[128];
			_mockLoader.Setup(x => x.ReadBytes(It.IsAny<UInt32>(), It.IsAny<UInt32>()))
				.Returns(testData);

			// Act
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				String[] strings = axmlFile.Strings;

				// Assert
				Assert.IsNotNull(strings);
				Assert.AreEqual(1, strings.Length);
			}
		}

		[Test]
		public void ReadLen16_WithSingleInt16_ReturnsCorrectLength()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader(1, false);
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			// Single Int16 length (no high bit set)
			_mockLoader.Setup(x => x.PtrToStructure<Int16>(It.IsAny<UInt32>()))
				.Returns((Int16)5);

			_mockLoader.Setup(x => x.PtrToStructure<UInt32>(It.IsAny<UInt32>()))
				.Returns(0U);

			Byte[] testData = Encoding.Unicode.GetBytes("Hello");
			_mockLoader.Setup(x => x.ReadBytes(It.IsAny<UInt32>(), It.IsAny<UInt32>()))
				.Returns(testData);

			// Act
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				String[] strings = axmlFile.Strings;

				// Assert
				Assert.AreEqual("Hello", strings[0]);
			}
		}

		[Test]
		public void ReadLen16_WithTwoInt16_ReturnsCorrectLength()
		{
			// Arrange
			AxmlApi.AxmlFileHeader header = CreateValidAxmlHeader(1, false);
			_mockLoader.Setup(x => x.PtrToStructure<AxmlApi.AxmlFileHeader>(0))
				.Returns(header);

			// Two Int16 length (high bit set in first Int16)
			_mockLoader.SetupSequence(x => x.PtrToStructure<Int16>(It.IsAny<UInt32>()))
				.Returns(unchecked((Int16)0x8001)) // High bit set
				.Returns((Int16)100);   // Second part

			_mockLoader.Setup(x => x.PtrToStructure<UInt32>(It.IsAny<UInt32>()))
				.Returns(0U);

			Byte[] testData = new Byte[200];
			_mockLoader.Setup(x => x.ReadBytes(It.IsAny<UInt32>(), It.IsAny<UInt32>()))
				.Returns(testData);

			// Act
			using (AxmlFile axmlFile = new AxmlFile(_mockLoader.Object))
			{
				String[] strings = axmlFile.Strings;

				// Assert
				Assert.IsNotNull(strings);
				Assert.AreEqual(1, strings.Length);
			}
		}

		private void SetupStringReading(String[] strings)
		{
			// Setup string offsets
			var offsetSequence = _mockLoader.SetupSequence(x => x.PtrToStructure<UInt32>(It.IsAny<UInt32>()));
			for (Int32 i = 0; i < strings.Length; i++)
			{
				offsetSequence.Returns((UInt32)(i * 20)); // Mock offsets
			}

			// Setup string lengths
			var lengthSequence = _mockLoader.SetupSequence(x => x.PtrToStructure<Int16>(It.IsAny<UInt32>()));
			foreach (String str in strings)
			{
				lengthSequence.Returns((Int16)str.Length);
			}

			// Setup string data
			var dataSequence = _mockLoader.SetupSequence(x => x.ReadBytes(It.IsAny<UInt32>(), It.IsAny<UInt32>()));
			foreach (String str in strings)
			{
				Byte[] data = Encoding.Unicode.GetBytes(str);
				dataSequence.Returns(data);
			}
		}
	}
}
