using System;
using System.Collections.Generic;
using System.ComponentModel;
using AlphaOmega.Debug.Arsc;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>
	/// Declares a broadcast receiver (a BroadcastReceiver subclass) as one of the application's components.
	/// Broadcast receivers enable applications to receive intents that are broadcast by the system or by other applications, even when other components of the application are not running.
	/// </summary>
	/// <remarks>
	/// There are two ways to make a broadcast receiver known to the system: One is declare it in the manifest file with this element.
	/// The other is to create the receiver dynamically in code and register it with the Context.registerReceiver() method.
	/// For more information about how to dynamically create receivers, see the BroadcastReceiver class description.
	/// </remarks>
	public class ApkReceiver : ApkNodeT<ApkApplication>, IApkIntentedNode
	{
		/// <summary>Whether or not the broadcast receiver is direct-boot aware; that is, whether or not it can run before the user unlocks the device.</summary>
		/// <remarks>During Direct Boot, a broadcast receiver in your application can only access the data that is stored in device protected storage.</remarks>
		[DefaultValue(false)]
		public Boolean DirectBootAware => base.GetBooleanAttribute("directBootAware").GetValueOrDefault(false);

		/// <summary>Whether or not the broadcast receiver can be instantiated by the system — "true" if it can be, and "false" if not.</summary>
		/// <remarks>
		/// The <see cref="ApkApplication"/> element has its own enabled attribute that applies to all application components, including broadcast receivers.
		/// The <see cref="ApkApplication"/> and <see cref="ApkReceiver"/> attributes must both be "true" for the broadcast receiver to be enabled. If either is "false", it is disabled; it cannot be instantiated.
		/// </remarks>
		[DefaultValue(true)]
		public Boolean Enabled => base.GetBooleanAttribute("enabled").GetValueOrDefault(true);

		/// <summary>
		/// Whether or not the broadcast receiver can receive messages from sources outside its application — "true" if it can, and "false" if not.
		/// If "false", the only messages the broadcast receiver can receive are those sent by components of the same application or applications with the same user ID.
		/// </summary>
		/// <remarks>
		/// The default value depends on whether the broadcast receiver contains intent filters.
		/// The absence of any filters means that it can be invoked only by Intent objects that specify its exact class name.
		/// This implies that the receiver is intended only for application-internal use (since others would not normally know the class name).
		/// So in this case, the default value is "false".
		/// On the other hand, the presence of at least one filter implies that the broadcast receiver is intended to receive intents broadcast by the system or other applications, so the default value is "true".
		/// 
		/// This attribute is not the only way to limit a broadcast receiver's external exposure.
		/// You can also use a permission to limit the external entities that can send it messages (see the permission attribute).
		/// </remarks>
		[DefaultValue(true)]
		public Boolean Exported => base.GetBooleanAttribute("exported").GetValueOrDefault(true);

		/// <summary>An icon representing the broadcast receiver</summary>
		/// <see cref="ApkApplication.Icon"/>
		/// <see cref="ApkIntentFilter.Icon"/>
		/// <remarks>
		/// This attribute must be set as a reference to a drawable resource containing the image definition.
		/// If it is not set, the icon specified for the application as a whole is used instead (see the <see cref="ApkApplication"/> element's <see cref="ApkApplication.Icon"/> attribute).
		/// 
		/// The broadcast receiver's icon — whether set here or by the <see cref="ApkApplication"/> element — is also the default icon for all the receiver's intent filters (see the <see cref="ApkIntentFilter"/> element's <see cref="ApkIntentFilter.Icon"/> attribute).
		/// </remarks>
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

		/// <summary>A user-readable label for the broadcast receiver</summary>
		/// <see cref="ApkApplication.Label"/>
		/// <see cref="ApkIntentFilter.Label"/>
		/// <remarks>
		/// If this attribute is not set, the label set for the application as a whole is used instead (see the <see cref="ApkApplication"/> element's <see cref="ApkApplication.Label"/> attribute).
		/// The broadcast receiver's label — whether set here or by the <see cref="ApkApplication"/> element — is also the default label for all the receiver's intent filters (see the <see cref="ApkIntentFilter"/> element's <see cref="ApkIntentFilter.Label"/> attribute).
		/// </remarks>
		public String Label
		{
			get
			{
				List<String> result = base.Node.GetAttribute("label");
				if(result == null)
					return base.ParentNode.Label;

				if(Int32.TryParse(result[0], out Int32 resourceId))
				{
					ResourceRow resource = base.GetResource(resourceId);
					if(resource != null)
						return resource.Value;
				}
				return result[0];
			}
		}

		/// <summary>The name of the class that implements the broadcast receiver, a subclass of BroadcastReceiver</summary>
		/// <remarks>
		/// This should be a fully qualified class name (such as, "com.example.project.ReportReceiver").
		/// However, as a shorthand, if the first character of the name is a period (for example, ". ReportReceiver"), it is appended to the package name specified in the <see cref="AndroidManifest"/> element.
		/// </remarks>
		public String Name
		{
			get
			{
				String result = base.Node.Attributes["name"][0];
				return result.StartsWith(".")
					? base.ParentNode.ParentNode.Package + result
					: result;
			}
		}

		/// <summary>The name of a permission that broadcasters must have to send a message to the broadcast receiver</summary>
		/// <remarks>
		/// If this attribute is not set, the permission set by the <see cref="ApkApplication"/> element's permission attribute applies to the broadcast receiver.
		/// If neither attribute is set, the receiver is not protected by a permission.
		/// </remarks>
		public String Permission
		{
			get
			{
				List<String> result = base.Node.GetAttribute("permission");
				return result == null
					? base.ParentNode.Permission
					: result[0];
			}
		}

		/// <summary>The name of the process in which the broadcast receiver should run</summary>
		/// <remarks>
		/// Normally, all components of an application run in the default process created for the application.
		/// It has the same name as the application package.
		/// The <see cref="ApkApplication"/> element's process attribute can set a different default for all components.
		/// But each component can override the default with its own process attribute, allowing you to spread your application across multiple processes.
		/// 
		/// If the name assigned to this attribute begins with a colon (':'), a new process, private to the application, is created when it's needed and the broadcast receiver runs in that process.
		/// If the process name begins with a lowercase character, the receiver will run in a global process of that name, provided that it has permission to do so.
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

		/// <summary>Specifies the types of intents that an activity, service, or broadcast receiver can respond to</summary>
		/// <remarks>
		/// An intent filter declares the capabilities of its parent component — what an activity or service can do and what types of broadcasts a receiver can handle.
		/// It opens the component to receiving intents of the advertised type, while filtering out those that are not meaningful for the component.
		/// </remarks>
		public IEnumerable<ApkIntentFilter> IntentFilter
		{
			get
			{
				foreach(XmlNode node in base.Node["intent-filter"])
					yield return new ApkIntentFilter(this, node);
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

		internal ApkReceiver(ApkApplication parentNode, XmlNode recieverNode)
			: base(parentNode, recieverNode)
		{ }
	}
}