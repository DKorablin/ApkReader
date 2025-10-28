using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AlphaOmega.Debug
{
	/// <summary>AndroidManifest xml node</summary>
	[DebuggerDisplay("Name={"+nameof(NodeName)+"}")]
	public class XmlNode
	{
		/// <summary>Parent node</summary>
		public XmlNode ParentNode { get; }

		/// <summary>Node name</summary>
		public String NodeName { get; }

		/// <summary>Child nodes</summary>
		public Dictionary<String, List<XmlNode>> ChildNodes { get; } = new Dictionary<String, List<XmlNode>>();

		/// <summary>Node attributes</summary>
		public Dictionary<String, List<String>> Attributes { get; } = new Dictionary<String, List<String>>();

		/// <summary>Collection of child nodes with specific name</summary>
		/// <param name="nodeName">Required node name</param>
		/// <returns>Required child nodes</returns>
		public IEnumerable<XmlNode> this[String nodeName]
		{
			get
			{
				if(this.ChildNodes.TryGetValue(nodeName, out List<XmlNode> childNodes))
					foreach(XmlNode node in childNodes)
						yield return node;
			}
		}

		internal XmlNode(XmlNode parentNode, String nodeName)
		{
			this.ParentNode = parentNode;
			this.NodeName = nodeName ?? throw new ArgumentNullException(nameof(nodeName));
		}

		internal void AddChildNode(XmlNode node)
		{
			if(!this.ChildNodes.TryGetValue(node.NodeName, out List<XmlNode> nodes))
			{
				nodes = new List<XmlNode>();
				this.ChildNodes.Add(node.NodeName, nodes);
			}
			nodes.Add(node);
		}

		internal void AddAttribute(String name, String value)
		{
			if(!this.Attributes.TryGetValue(name,out List<String> attributes))
			{
				attributes = new List<String>();
				this.Attributes.Add(name, attributes);
			}
			attributes.Add(value);
		}

		/// <summary>Gets attribute values if attribute contains in the collection. Or null</summary>
		/// <param name="attributeName">Name of the attribute</param>
		/// <returns>Attribute values(s) if manifest contains multiple attribute with the same name in the node</returns>
		public List<String> GetAttribute(String attributeName)
			=> this.Attributes.TryGetValue(attributeName, out List<String> result)
				? result
				: null;

		/// <summary>Convert oop XML to string XML</summary>
		/// <returns>String xml representation</returns>
		public String ConvertToString()
			=> this.ConvertToString(0);

		/// <summary>Convert oop XML to string XML</summary>
		/// <param name="intendCount">Intent index</param>
		/// <returns>String xml representation</returns>
		private String ConvertToString(Int32 intendCount)
		{
			const Char IntentChar = '\t';

			StringBuilder attributes = new StringBuilder();
			foreach(var item in this.Attributes)
				attributes.Append(" " + item.Key + "=\"" + String.Join(",", item.Value.ToArray()) + "\"");

			String intent = new String(Array.ConvertAll(new Char[intendCount], delegate (Char n) { return IntentChar; }));
			StringBuilder result = new StringBuilder();
			result.AppendLine(intent + "<" + this.NodeName + attributes.ToString() + ">");
			foreach(var nodes in this.ChildNodes)
				foreach(XmlNode childNode in nodes.Value)
					result.Append(childNode.ConvertToString(intendCount + 1));
			result.AppendLine(intent + "</" + this.NodeName + ">");

			return result.ToString();
		}
	}
}