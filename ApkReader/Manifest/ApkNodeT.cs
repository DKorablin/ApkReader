
namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Generic XML node of ApkManifest.xml file</summary>
	/// <typeparam name="T">XML node</typeparam>
	public class ApkNodeT<T> : ApkNode where T : ApkNode
	{
		private readonly T _parentNode;

		/// <summary>Parent generic XML node</summary>
		public T ParentNode { get { return this._parentNode; } }

		/// <summary>.ctor generic XML node</summary>
		/// <param name="parentNode">Parent generic XML node</param>
		/// <param name="node">XML node</param>
		public ApkNodeT(T parentNode,XmlNode node)
			: base(parentNode, node)
		{
			this._parentNode = parentNode;
		}
	}
}