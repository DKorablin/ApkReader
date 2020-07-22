using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Defines the path and required permissions for a specific subset of data within a content provider.</summary>
	/// <remarks>This element can be specified multiple times to supply multiple paths.</remarks>
	public class ApkPathPermission : ApkNodeT<ApkProvider>
	{
		/// <summary>A complete URI path for a subset of content provider data</summary>
		/// <remarks>
		/// Permission can be granted only to the particular data identified by this path.
		/// When used to provide search suggestion content, it must be appended with "/search_suggest_query".
		/// </remarks>
		public String Path
		{
			get
			{
				List<String> result = base.Node.GetAttribute("path");
				return result == null
					? null
					: result[0];
			}
		}

		/// <summary>The initial part of a URI path for a subset of content provider data.</summary>
		/// <remarks>Permission can be granted to all data subsets with paths that share this initial part.</remarks>
		public String PathPrefix
		{
			get
			{
				List<String> result = base.Node.GetAttribute("pathPrefix");
				return result == null
					? null
					: result[0];
			}
		}

		/// <summary>
		/// A complete URI path for a subset of content provider data, but one that can use the following wildcards:
		/// An asterisk ('*'). This matches a sequence of 0 to many occurrences of the immediately preceding character.
		/// A period followed by an asterisk (".*"). This matches any sequence of 0 or more characters.
		/// </summary>
		/// <remarks>
		/// Because '\' is used as an escape character when the string is read from XML (before it is parsed as a pattern), you will need to double-escape.
		/// For example, a literal '*' would be written as "\\*" and a literal '\' would be written as "\\".
		/// This is basically the same as what you would need to write if constructing the string in Java code.
		/// </remarks>
		public String PathPattern
		{
			get
			{
				List<String> result = base.Node.GetAttribute("pathPattern");
				return result == null
					? null
					: result[0];
			}
		}

		/// <summary>The name of a permission that clients must have in order to read or write the content provider's data</summary>
		/// <see cref="ReadPermission"/>
		/// <see cref="WritePermission"/>
		/// <remarks>
		/// This attribute is a convenient way of setting a single permission for both reading and writing.
		/// However, the readPermission and writePermission attributes take precedence over this one.
		/// </remarks>
		public String Permission
		{
			get
			{
				List<String> result = base.Node.GetAttribute("permission");
				return result == null
					? null
					: result[0];
			}
		}

		/// <summary>A permission that clients must have in order to query the content provider</summary>
		public String ReadPermission
		{
			get
			{
				List<String> result = base.Node.GetAttribute("readPermission");
				return result == null
					? null
					: result[0];
			}
		}

		/// <summary>A permission that clients must have in order to make changes to the data controlled by the content provider</summary>
		public String WritePermission
		{
			get
			{
				List<String> result = base.Node.GetAttribute("writePermission");
				return result == null
					? null
					: result[0];
			}
		}

		internal ApkPathPermission(ApkProvider parentNode,XmlNode node)
			: base(parentNode, node)
		{

		}
	}
}