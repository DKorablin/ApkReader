
namespace AlphaOmega.Debug.Axml
{
	/// <summary>Android XML chunk</summary>
	public class AxmlChunk
	{
		private readonly AxmlApi.Chunk _chunk;
		private readonly AxmlApi.Document? _document;
		private readonly AxmlApi.StartTag? _sTag;
		private readonly AxmlApi.EndTag? _eTag;
		private readonly AxmlApi.Text _text;
		private readonly AxmlApi.Attribute[] _attributes;

		/// <summary>Chunk description</summary>
		public AxmlApi.Chunk Chunk { get { return this._chunk; } }

		/// <summary>Document node</summary>
		public AxmlApi.Document? Document { get { return this._document; } }

		/// <summary>Tag begin node</summary>
		public AxmlApi.StartTag? StartTag { get { return this._sTag; } }

		/// <summary>Tag end node</summary>
		public AxmlApi.EndTag? EndTag { get { return this._eTag; } }

		/// <summary>Text xml node</summary>
		public AxmlApi.Text? Text { get { return this._text; } }

		/// <summary>Tag begin attributes</summary>
		public AxmlApi.Attribute[] Attributes { get { return this._attributes; } }

		private AxmlChunk(AxmlApi.Chunk chunk)
		{
			this._chunk = chunk;
		}

		/// <summary>Create instance of andoid xml cunk with document node</summary>
		/// <param name="chunk">axml chunk</param>
		/// <param name="document">Document node</param>
		public AxmlChunk(AxmlApi.Chunk chunk, AxmlApi.Document document)
			: this(chunk)
		{
			this._document = document;
		}

		/// <summary>Create instance of andoid xml cunk with document node</summary>
		/// <param name="chunk">axml chunk</param>
		/// <param name="tag">Tag begin node</param>
		/// <param name="attributes">tag nodes</param>
		public AxmlChunk(AxmlApi.Chunk chunk, AxmlApi.StartTag tag, AxmlApi.Attribute[] attributes)
			: this(chunk)
		{
			this._sTag = tag;
			this._attributes = attributes;
		}

		/// <summary>Create instance of android xml chunk with end tag node</summary>
		/// <param name="chunk">axml chunk</param>
		/// <param name="tag">Tag end node</param>
		public AxmlChunk(AxmlApi.Chunk chunk, AxmlApi.EndTag tag)
			: this(chunk)
		{
			this._eTag = tag;
		}

		/// <summary>Create instance of of android xml chunk with text node</summary>
		/// <param name="chunk">axml chunk</param>
		/// <param name="text">Text node</param>
		public AxmlChunk(AxmlApi.Chunk chunk, AxmlApi.Text text)
			: this(chunk)
		{
			this._text = text;
		}
	}
}