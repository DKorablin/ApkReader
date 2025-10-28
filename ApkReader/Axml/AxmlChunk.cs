
namespace AlphaOmega.Debug.Axml
{
	/// <summary>Android XML chunk</summary>
	public class AxmlChunk
	{
		/// <summary>Chunk description</summary>
		public AxmlApi.Chunk Chunk { get; }

		/// <summary>Document node</summary>
		public AxmlApi.Document? Document { get; }

		/// <summary>Tag begin node</summary>
		public AxmlApi.StartTag? StartTag { get; }

		/// <summary>Tag end node</summary>
		public AxmlApi.EndTag? EndTag { get; }

		/// <summary>Text xml node</summary>
		public AxmlApi.Text? Text { get; }

		/// <summary>Tag begin attributes</summary>
		public AxmlApi.Attribute[] Attributes { get; }

		private AxmlChunk(AxmlApi.Chunk chunk)
			=> this.Chunk = chunk;

		/// <summary>Create instance of Android xml cunk with document node</summary>
		/// <param name="chunk">axml chunk</param>
		/// <param name="document">Document node</param>
		public AxmlChunk(AxmlApi.Chunk chunk, AxmlApi.Document document)
			: this(chunk)
			=> this.Document = document;

		/// <summary>Create instance of Android xml cunk with document node</summary>
		/// <param name="chunk">axml chunk</param>
		/// <param name="tag">Tag begin node</param>
		/// <param name="attributes">tag nodes</param>
		public AxmlChunk(AxmlApi.Chunk chunk, AxmlApi.StartTag tag, AxmlApi.Attribute[] attributes)
			: this(chunk)
		{
			this.StartTag = tag;
			this.Attributes = attributes;
		}

		/// <summary>Create instance of Android xml chunk with end tag node</summary>
		/// <param name="chunk">axml chunk</param>
		/// <param name="tag">Tag end node</param>
		public AxmlChunk(AxmlApi.Chunk chunk, AxmlApi.EndTag tag)
			: this(chunk)
			=> this.EndTag = tag;

		/// <summary>Create instance of of Android xml chunk with text node</summary>
		/// <param name="chunk">axml chunk</param>
		/// <param name="text">Text node</param>
		public AxmlChunk(AxmlApi.Chunk chunk, AxmlApi.Text text)
			: this(chunk)
			=> this.Text = text;
	}
}