using System;
using System.Collections.Generic;
using System.ComponentModel;
using AlphaOmega.Debug.Arsc;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Declares a service (a Service subclass) as one of the application's components</summary>
	/// <remarks>
	/// Unlike activities, services lack a visual user interface.
	/// They're used to implement long-running background operations or a rich communications API that can be called by other applications.
	/// 
	/// All services must be represented by <see cref="ApkService"/> elements in the manifest file. Any that are not declared there will not be seen by the system and will never be run.
	/// </remarks>
	public class ApkService : ApkNodeT<ApkApplication>, IApkIntentedNode
	{
		/// <summary>Specify that the service is a foreground service that satisfies a particular use case</summary>
		public enum ForegroundService
		{
			/// <summary>banana banana banana</summary>
			connectedDevice,
			/// <summary>banana banana banana</summary>
			dataSync,
			/// <summary>banana banana banana</summary>
			location,
			/// <summary>banana banana banana</summary>
			mediaPlayback,
			/// <summary>banana banana banana</summary>
			mediaProjection,
			/// <summary>banana banana banana</summary>
			phoneCall,
		}

		/// <summary>A string that describes the service to users</summary>
		/// <remarks>The label should be set as a reference to a string resource, so that it can be localized like other strings in the user interface.</remarks>
		public String Description
		{
			get
			{
				List<String> result = base.Node.GetAttribute("description");
				return result == null
					? null
					: base.GetResource(Convert.ToInt32(result[0])).Value;
			}
		}

		/// <summary>Whether or not the service is direct-boot aware; that is, whether or not it can run before the user unlocks the device.</summary>
		/// <remarks>During Direct Boot, a service in your application can only access the data that is stored in device protected storage.</remarks>
		[DefaultValue(false)]
		public Boolean DirectBootAware
			=> base.GetBooleanAttribute("directBootAware").GetValueOrDefault(false);

		/// <summary>Whether or not the service can be instantiated by the system — "true" if it can be, and "false" if not</summary>
		/// <remarks>
		/// The <see cref="ApkApplication"/> element has its own enabled attribute that applies to all application components, including services.
		/// The <see cref="ApkApplication"/> and <see cref="ApkService"/> attributes must both be "true" (as they both are by default) for the service to be enabled.
		/// If either is "false", the service is disabled; it cannot be instantiated
		/// </remarks>
		[DefaultValue(true)]
		public Boolean Enabled
			=> base.GetBooleanAttribute("enabled").GetValueOrDefault(true);

		/// <summary>
		/// Whether or not components of other applications can invoke the service or interact with it — "true" if they can, and "false" if not.
		/// When the value is "false", only components of the same application or applications with the same user ID can start the service or bind to it.
		/// </summary>
		/// <remarks>
		/// The default value depends on whether the service contains intent filters.
		/// The absence of any filters means that it can be invoked only by specifying its exact class name.
		/// This implies that the service is intended only for application-internal use (since others would not know the class name).
		/// So in this case, the default value is "false".
		/// On the other hand, the presence of at least one filter implies that the service is intended for external use, so the default value is "true".
		/// 
		/// This attribute is not the only way to limit the exposure of a service to other applications.
		/// You can also use a permission to limit the external entities that can interact with the service (see the permission attribute).
		/// </remarks>
		//[DefaultValue(false)]
		public Boolean? Exported//TODO: Тут необходимо предусмотреть условие по фильтрам. Ибо от них зависит true или false
			=> base.GetBooleanAttribute("exported");

		/// <summary>
		/// Specify that the service is a foreground service that satisfies a particular use case.
		/// For example, a foreground service type of "location" indicates that an app is getting the device's current location, usually to continue a user-initiated action related to device location.
		/// </summary>
		/// <remarks>You can assign multiple foreground service types to a particular service</remarks>
		public ForegroundService[] ForegroundServiceType
		{
			get
			{
				List<String> result = base.Node.GetAttribute("foregroundServiceType");
				return result == null
					? new ForegroundService[] { }
					: Array.ConvertAll(result[0].Split('|'), delegate(String item) { return (ForegroundService)Enum.Parse(typeof(ForegroundService), item); });
			}
		}

		/// <summary>An icon representing the service</summary>
		/// <remarks>
		/// The service's icon — whether set here or by the <see cref="ApkApplication"/> element — is also the default icon for all the service's intent filters (see the <see cref="ApkIntentFilter"/> element's icon attribute).
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

		/// <summary>
		/// If set to true, this service will run under a special process that is isolated from the rest of the system and has no permissions of its own.
		/// The only communication with it is through the Service API (binding and starting).
		/// </summary>
		public Boolean? IsolatedProcess => base.GetBooleanAttribute("isolatedProcess");

		/// <summary>
		/// A name for the service that can be displayed to users.
		/// If this attribute is not set, the label set for the application as a whole is used instead (see the <see cref="ApkApplication"/> element's <see cref="ApkApplication.Label"/> attribute).
		/// </summary>
		/// <see cref="ApkApplication.Label"/>
		/// <see cref="ApkIntentFilter.Label"/>
		/// <remarks>
		/// The service's label — whether set here or by the <see cref="ApkApplication"/> element — is also the default label for all the service's intent filters (see the <see cref="ApkIntentFilter"/> element's <see cref="ApkIntentFilter.Label"/> attribute).
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

		/// <summary>The name of the Service subclass that implements the service</summary>
		/// <remarks>Once you publish your application, you should not change this name (unless you've set android:exported="false").</remarks>
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

		/// <summary>
		/// The name of a permission that an entity must have in order to launch the service or bind to it.
		/// If a caller of startService(), bindService(), or stopService(), has not been granted this permission, the method will not work and the Intent object will not be delivered to the service.
		/// </summary>
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

		/// <summary>
		/// The name of the process where the service is to run.
		/// Normally, all components of an application run in the default process created for the application.
		/// It has the same name as the application package.
		/// The <see cref="ApkApplication"/> element's process attribute can set a different default for all components.
		/// But component can override the default with its own process attribute, allowing you to spread your application across multiple processes.
		/// </summary>
		/// <remarks>
		/// If the name assigned to this attribute begins with a colon (':'), a new process, private to the application, is created when it's needed and the service runs in that process.
		/// If the process name begins with a lowercase character, the service will run in a global process of that name, provided that it has permission to do so.
		/// This allows components in different applications to share a process, reducing resource usage.
		/// </remarks>
		public String Process
		{
			get
			{
				List<String> result = base.Node.GetAttribute("process");
				return result?[0];
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

		internal ApkService(ApkApplication parentNode,XmlNode node)
			: base(parentNode, node)
		{ }
	}
}