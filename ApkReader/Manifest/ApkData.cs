using System;
using System.Collections.Generic;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Adds a data specification to an intent filter</summary>
	/// <remarks>
	/// The specification can be just a data type (the mimeType attribute), just a URI, or both a data type and a URI.
	/// A URI is specified by separate attributes for each of its parts:
	/// {scheme}://{host}:{port}[{path}|{pathPrefix}|{pathPattern}]
	/// </remarks>
	public class ApkData : ApkNodeT<ApkIntentFilter>
	{
		/// <summary>The scheme part of a URI</summary>
		/// <remarks>A scheme is specified without the trailing colon (for example, http, rather than http:)</remarks>
		public String Scheme
		{
			get { return this.GetAttributeValue("scheme"); }
		}

		/// <summary>The host part of a URI authority</summary>
		/// <remarks>
		///  This attribute is meaningless unless a scheme attribute is also specified for the filter.
		///  To match multiple subdomains, use an asterisk (*) to match zero or more characters in the host.
		///  For example, the host *.google.com matches www.google.com, .google.com, and developer.google.com.
		/// </remarks>
		public String Host
		{
			get { return this.GetAttributeValue("host"); }
		}

		/// <summary>The port part of a URI authority</summary>
		/// <remarks> This attribute is meaningful only if the scheme and host attributes are also specified for the filter.</remarks>
		public String Port
		{
			get { return this.GetAttributeValue("port"); }
		}

		/// <summary>The path part of a URI which must begin with a /.</summary>
		/// <remarks>The path attribute specifies a complete path that is matched against the complete path in an Intent object.</remarks>
		public String Path
		{
			get { return this.GetAttributeValue("path"); }
		}

		/// <summary>
		/// The pathPattern attribute specifies a complete path that is matched against the complete path in the Intent object, but it can contain the following wildcards:
		/// An asterisk ('*') matches a sequence of 0 to many occurrences of the immediately preceding character.
		/// A period followed by an asterisk (".*") matches any sequence of 0 to many characters.
		/// </summary>
		public String PathPattern
		{
			get { return this.GetAttributeValue("pathPattern"); }
		}

		/// <summary>The pathPrefix attribute specifies a partial path that is matched against only the initial part of the path in the Intent object.</summary>
		public String PathPrefix
		{
			get { return this.GetAttributeValue("pathPrefix"); }
		}

		/// <summary>A MIME media type, such as image/jpeg or audio/mpeg4-generic</summary>
		/// <remarks>The subtype can be the asterisk wildcard (*) to indicate that any subtype matches.</remarks>
		public String MimeType
		{
			get { return this.GetAttributeValue("mimeType"); }
		}

		internal ApkData(ApkIntentFilter parentNode,XmlNode node)
			: base(parentNode, node)
		{
		}

		/// <summary>Convert data attribute to Uri string if available</summary>
		/// <returns>String represetation of data node</returns>
		public String ToUri()
		{
			if(this.Scheme == null)
				return null;

			if(this.Host == null)
				return this.Scheme;

			return $"{this.Scheme}://{this.Host}:{this.Port}{this.Path}{this.PathPrefix}{this.PathPattern}";
		}

		private String GetAttributeValue(String attibuteName)
		{
			List<String> result = base.Node.GetAttribute(attibuteName);
			return result == null
				? null
				: result[0];
		}
	}
}