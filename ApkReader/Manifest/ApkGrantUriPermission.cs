using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Specifies the subsets of app data that the parent content provider has permission to access</summary>
	/// <remarks>
	/// Data subsets are indicated by the path part of a content: URI.
	/// (The authority part of the URI identifies the content provider.)
	/// Granting permission is a way of enabling clients of the provider that don't normally have permission to access its data to overcome that restriction on a one-time basis.
	/// 
	/// If a content provider's grantUriPermissions attribute is "true", permission can be granted for any the data under the provider's purview.
	/// However, if that attribute is "false", permission can be granted only to data subsets that are specified by this element.
	/// A provider can contain any number of <see cref="ApkGrantUriPermission"/> elements.
	/// Each one can specify only one path (only one of the three possible attributes).
	/// </remarks>
	public class ApkGrantUriPermission : ApkNodeT<ApkProvider>
	{
		/// <summary> The path attribute specifies a complete path; permission can be granted only to the particular data subset identified by that path.</summary>
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

		/// <summary>
		/// The pathPattern attribute specifies a complete path, but one that can contain the following wildcards:
		/// An asterisk ('*') matches a sequence of 0 to many occurrences of the immediately preceding character.
		/// A period followed by an asterisk (".*") matches any sequence of 0 to many characters.
		/// </summary>
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

		/// <summary>The pathPrefix attribute specifies the initial part of a path; permission can be granted to all data subsets with paths that share that initial part.</summary>
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
		internal ApkGrantUriPermission(ApkProvider parentNode, XmlNode node)
			: base(parentNode, node)
		{
		}
	}
}