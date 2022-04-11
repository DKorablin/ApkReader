using System;
using System.Collections.Generic;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>A name-value pair for an item of additional, arbitrary data that can be supplied to the parent component.</summary>
	/// <remarks>
	/// A component element can contain any number of <see cref="ApkMetaData"/> subelements.
	/// The values from all of them are collected in a single Bundle object and made available to the component as the PackageItemInfo.metaData field.
	/// </remarks>
	public class ApkMetaData : ApkNode
	{
		/// <summary>A unique name for the item</summary>
		/// <remarks> To ensure that the name is unique, use a Java-style naming convention — for example, "com.example.project.activity.fred".</remarks>
		public String Name
		{
			get { return base.Node.Attributes["name"][0]; }
		}

		/// <summary>A reference to a resource</summary>
		/// <remarks>
		///  The ID of the resource is the value assigned to the item.
		///  The ID can be retrieved from the meta-data Bundle by the Bundle.getInt() method.
		///  </remarks>
		public String Resource
		{
			get
			{
				List<String> result = base.Node.GetAttribute("resource");
				if(result == null)
					return null;

				Arsc.ResourceRow resource = base.GetResource(Convert.ToInt32(result[0]));
				return resource == null
					? null
					: resource.Value;
			}
		}

		/// <summary>The value assigned to the item</summary>
		/// <remarks> The data types that can be assigned as values and the Bundle methods that components use to retrieve those values</remarks>
		public String Value
		{
			get
			{
				List<String> result = base.Node.GetAttribute("value");
				return result == null
					? null
					: result[0];
			}
		}

		internal ApkMetaData(ApkNode parentNode, XmlNode node)
			: base(parentNode, node)
		{

		}
	}
}