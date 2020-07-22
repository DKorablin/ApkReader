using System;
using System.Collections.Generic;
using System.ComponentModel;
using AlphaOmega.Debug.Arsc;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Declares a content provider component</summary>
	/// <remarks>
	/// A content provider is a subclass of ContentProvider that supplies structured access to data managed by the application.
	/// All content providers in your application must be defined in a <see cref="ApkProvider"/> element in the manifest file; otherwise, the system is unaware of them and doesn't run them.
	/// </remarks>
	public class ApkProvider : ApkNodeT<ApkApplication>
	{
		/// <summary>A list of one or more URI authorities that identify data offered by the content provider.</summary>
		/// <remarks>
		///  To avoid conflicts, authority names should use a Java-style naming convention (such as com.example.provider.cartoonprovider).
		///  Typically, it's the name of the ContentProvider subclass that implements the provider.
		/// </remarks>
		public String[] Authorities
		{
			get { return base.Node.Attributes["authorities"][0].Split(';'); }
		}

		/// <summary>Whether or not the content provider can be instantiated by the system — "true" if it can be, and "false" if not</summary>
		/// <remarks>
		/// The <see cref="ApkApplication"/> element has its own enabled attribute that applies to all application components, including content providers.
		/// The <see cref="ApkApplication"/> and <see cref="ApkProvider"/> attributes must both be "true" (as they both are by default) for the content provider to be enabled.
		/// If either is "false", the provider is disabled; it cannot be instantiated.
		/// </remarks>
		[DefaultValue(true)]
		public Boolean Enabled
		{
			get { return base.GetBooleanAttribute("enabled").GetValueOrDefault(true); }
		}

		/// <summary>Whether or not the content provider is direct-boot aware; that is, whether or not it can run before the user unlocks the device.</summary>
		/// <remarks>During Direct Boot, a content provider in your application can only access the data that is stored in device protected storage.</remarks>
		[DefaultValue(false)]
		public Boolean DirectBootAware
		{
			get { return base.GetBooleanAttribute("directBootAware").GetValueOrDefault(false); }
		}

		/// <summary>
		/// Whether the content provider is available for other applications to use: 
		/// true: The provider is available to other applications. Any application can use the provider's content URI to access it, subject to the permissions specified for the provider.
		/// false: The provider is not available to other applications. Set android:exported="false" to limit access to the provider to your applications.
		/// 
		/// Only applications that have the same user ID (UID) as the provider, or applications that have been temporarily granted access to the provider through the android:grantUriPermissions element, have access to it.
		/// </summary>
		public Boolean? Exported
		{
			get { return base.GetBooleanAttribute("exported"); }
		}

		/// <summary>
		/// Whether or not those who ordinarily would not have permission to access the content provider's data can be granted permission to do so, temporarily overcoming the restriction imposed by the readPermission, writePermission, permission, and exported attributes — "true" if permission can be granted, and "false" if not.
		/// If "true", permission can be granted to any of the content provider's data.
		/// If "false", permission can be granted only to the data subsets listed in <see cref="ApkGrantUriPermission"/> subelements, if any.
		/// </summary>
		/// <remarks>
		/// Granting permission is a way of giving an application component one-time access to data protected by a permission.
		/// For example, when an e-mail message contains an attachment, the mail application may call upon the appropriate viewer to open it, even though the viewer doesn't have general permission to look at all the content provider's data.
		/// </remarks>
		[DefaultValue(false)]
		public Boolean GrantUriPermissions
		{
			get { return base.GetBooleanAttribute("grantUriPermissions").GetValueOrDefault(false); }
		}

		/// <summary>An icon representing the content provider</summary>
		public String Icon
		{
			get
			{
				List<String> result = base.Node.GetAttribute("icon");
				return result == null
					? base.ParentNode.Icon
					: base.GetResource(Convert.ToInt32(result[0])).Value;
			}
		}

		/// <summary>
		/// The order in which the content provider should be instantiated, relative to other content providers hosted by the same process.
		/// When there are dependencies among content providers, setting this attribute for each of them ensures that they are created in the order required by those dependencies.
		/// </summary>
		public Int32? InitOrder
		{
			get
			{
				List<String> result = base.Node.GetAttribute("initOrder");
				return result == null
					? (Int32?)null
					: Convert.ToInt32(result[0]);
			}
		}

		/// <summary>A user-readable label for the content provided</summary>
		public String Label
		{
			get
			{
				List<String> result = base.Node.GetAttribute("label");
				if(result == null)
					return base.ParentNode.Label;

				Int32 resourceId;
				if(Int32.TryParse(result[0], out resourceId))
				{
					ResourceRow resource = base.GetResource(resourceId);
					if(resource != null)
						return resource.Value;
				}
				return result[0];
			}
		}

		/// <summary>
		/// If the app runs in multiple processes, this attribute determines whether multiple instances of the content provider are created.
		/// If true, each of the app's processes has its own content provider object.
		/// If false, the app's processes share only one content provider object.
		/// </summary>
		/// <remarks>Setting this flag to true may improve performance by reducing the overhead of interprocess communication, but it also increases the memory footprint of each process.</remarks>
		[DefaultValue(false)]
		public Boolean Multiprocess
		{
			get { return base.GetBooleanAttribute("multiprocess").GetValueOrDefault(false); }
		}

		/// <summary>The name of the class that implements the content provider, a subclass of ContentProvider</summary>
		public String Name
		{
			get
			{
				String name = base.Node.Attributes["name"][0];
				return name[0] == '.'
					? base.ParentNode.ParentNode.Package + name
					: name;
			}
		}

		/// <summary>
		/// The name of a permission that clients must have to read or write the content provider's data.
		/// This attribute is a convenient way of setting a single permission for both reading and writing.
		/// </summary>
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

		/// <summary>
		/// The name of the process in which the content provider should run.
		/// Normally, all components of an application run in the default process created for the application.
		/// </summary>
		/// <remarks>
		/// If the name assigned to this attribute begins with a colon (':'), a new process, private to the application, is created when it's needed and the activity runs in that process.
		/// If the process name begins with a lowercase character, the activity will run in a global process of that name, provided that it has permission to do so.
		/// This allows components in different applications to share a process, reducing resource usage.
		/// </remarks>
		public String Process
		{
			get
			{
				List<String> result = base.Node.GetAttribute("process");
				return result == null
					? base.ParentNode.Process
					: result[0];
			}
		}

		/// <summary>A permission that clients must have to query the content provider.</summary>
		/// <remarks>If the provider sets android:grantUriPermissions to true, or if a given client satisfies the conditions of a <see cref="ApkGrantUriPermission"/> subelement, the client can gain temporary read access to the content provider's data.</remarks>
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

		/// <summary>Whether or not the data under the content provider's control is to be synchronized with data on a server — "true" if it is to be synchronized, and "false" if not.</summary>
		public Boolean? Syncable
		{
			get { return base.GetBooleanAttribute("syncable"); }
		}

		/// <summary>A permission that clients must have to make changes to the data controlled by the content provider.</summary>
		/// <remarks>If the provider sets android:grantUriPermissions to true, or if a given client satisfies the conditions of a <see cref="ApkGrantUriPermission"/> subelement, the client can gain temporary write access to modify the content provider's data.</remarks>
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

		/// <summary>A name-value pair for an item of additional, arbitrary data that can be supplied to the parent component.</summary>
		/// <remarks>
		/// A component element can contain any number of <see cref="ApkMetaData"/> subelements.
		/// The values from all of them are collected in a single Bundle object and made available to the component as the PackageItemInfo.metaData field.
		/// </remarks>
		public IEnumerable<ApkMetaData> MetaData
		{
			get
			{
				foreach(XmlNode node in base.Node["meta-data"])
					yield return new ApkMetaData(this, node);
			}
		}

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
		public IEnumerable<ApkGrantUriPermission> GrantUriPermission
		{
			get
			{
				foreach(XmlNode node in base.Node["grant-uri-permission"])
					yield return new ApkGrantUriPermission(this, node);
			}
		}

		/// <summary>Defines the path and required permissions for a specific subset of data within a content provider.</summary>
		/// <remarks>This element can be specified multiple times to supply multiple paths.</remarks>
		public IEnumerable<ApkPathPermission> PathPermission
		{
			get
			{
				foreach(XmlNode node in base.Node["path-permission"])
					yield return new ApkPathPermission(this, node);
			}
		}

		internal ApkProvider(ApkApplication parentNode,XmlNode node)
			: base(parentNode, node)
		{
		}
	}
}