using System;
using System.Collections.Generic;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>XML node of ApkManifest.xml file</summary>
	public class ApkNode
	{
		private readonly Dictionary<Int32, List<Arsc.ResourceRow>> _resourceMap;
		private readonly XmlNode _node;

		/// <summary>Current XML node</summary>
		public XmlNode Node { get { return this._node; } }

		/// <summary>resources.arsc reference</summary>
		protected Dictionary<Int32, List<Arsc.ResourceRow>> ResourceMap { get { return this._resourceMap; } }

		/// <summary>.ctor XML node</summary>
		/// <param name="parentNode">Parent XML node</param>
		/// <param name="node">XML node</param>
		protected ApkNode(ApkNode parentNode, XmlNode node)
			: this(node, parentNode._resourceMap)
		{

		}

		/// <summary>.ctor XML node</summary>
		/// <param name="node">XML node</param>
		/// <param name="resourceMap">resource.arsc reference</param>
		protected ApkNode(XmlNode node, Dictionary<Int32, List<Arsc.ResourceRow>> resourceMap)
		{
			this._node = node ?? throw new ArgumentNullException(nameof(node));
			this._resourceMap = resourceMap;
		}

		/// <summary>Gets resource reference from resource map</summary>
		/// <param name="resourceId">ID of required resource</param>
		/// <returns>Returns resource reference</returns>
		protected Arsc.ResourceRow GetResource(Int32 resourceId)
		{
			List<Arsc.ResourceRow> value;
			return this.ResourceMap.TryGetValue(resourceId, out value)
				? value[0]
				: null;
		}

		/// <summary>Get Boolean attribute from XML node</summary>
		/// <param name="attributeName">Name of attribute</param>
		/// <returns>Boolean value or null in undefined or unknown value</returns>
		protected Boolean? GetBooleanAttribute(String attributeName)
		{
			List<String> result = this.Node.GetAttribute(attributeName);
			if(result == null)
				return null;

			switch(result[0])
			{
			case "true":
				return true;
			case "false":
				return false;
			default:
				return null;
			}
		}
	}
}