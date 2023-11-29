
namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Generic XML node of ApkManifest.xml file</summary>
	/// <typeparam name="T">XML node</typeparam>
	public class ApkNodeT<T> : ApkNode where T : ApkNode
	{
		/// <summary>Parent generic XML node</summary>
		public T ParentNode { get; }

		/// <summary>.ctor generic XML node</summary>
		/// <param name="parentNode">Parent generic XML node</param>
		/// <param name="node">XML node</param>
		public ApkNodeT(T parentNode,XmlNode node)
			: base(parentNode, node)
			=> this.ParentNode = parentNode;
	}
}