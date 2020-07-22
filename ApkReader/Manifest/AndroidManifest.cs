using System;
using System.Collections.Generic;
using System.ComponentModel;
using AlphaOmega.Debug.Arsc;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>
	/// The root element of the AndroidManifest.xml file.
	/// It must contain an <see cref="ApkApplication"/> element and specify xmlns:android and package attributes.
	/// </summary>
	public class AndroidManifest : ApkNode
	{//https://developer.android.com/guide/topics/manifest

		/// <summary>The default install location for the app</summary>
		public enum InstallLocationType
		{
			/// <summary>
			/// The app must be installed on the internal device storage only.
			/// If this is set, the app will never be installed on the external storage.
			/// If the internal storage is full, then the system will not install the app.
			/// This is also the default behavior if you do not define android:installLocation.
			/// </summary>
			internalOnly,

			/// <summary>
			/// The app may be installed on the external storage, but the system will install the app on the internal storage by default.
			/// If the internal storage is full, then the system will install it on the external storage.
			/// Once installed, the user can move the app to either internal or external storage through the system settings.
			/// </summary>
			auto,

			/// <summary>
			/// The app prefers to be installed on the external storage (SD card).
			/// There is no guarantee that the system will honor this request.
			/// The app might be installed on internal storage if the external media is unavailable or full.
			/// Once installed, the user can move the app to either internal or external storage through the system settings.
			/// </summary>
			preferExternal,
		}

		/// <summary>A full Java-language-style package name for the Android app</summary>
		/// <remarks>
		/// The name may contain uppercase or lowercase letters ('A' through 'Z'), numbers, and underscores ('_').
		/// However, individual package name parts may only start with letters.
		/// 
		/// This name is also the default name for your app process (see the <see cref="ApkApplication"/> element's process attribute).
		/// And it's the default task affinity for your activities (see the <see cref="ApkActivity"/> element's taskAffinity attribute).
		/// </remarks>
		public String Package { get { return base.Node.Attributes["package"][0]; } }

		/// <summary>The name of a Linux user ID that will be shared with other apps</summary>
		/// <remarks>
		/// By default, Android assigns each app its own unique user ID.
		/// However, if this attribute is set to the same value for two or more apps, they will all share the same ID — provided that their certificate sets are identical.
		/// Apps with the same user ID can access each other's data and, if desired, run in the same process.
		/// </remarks>
		[Obsolete("This constant was deprecated in API level 29", false)]
		public String SharedUserId
		{
			get
			{
				List<String> sharedUserId = base.Node.GetAttribute("sharedUserId");
				return sharedUserId == null
					? null
					: sharedUserId[0];
			}
		}

		/// <summary>The target sandbox for this app to use</summary>
		/// <remarks>
		/// The higher the sandbox version number, the higher the level of security.
		/// Its default value is 1; you can also set it to 2. Setting this attribute to 2 switches the app to a different SELinux sandbox.
		/// </remarks>
		[DefaultValue(1)]
		public Int32 TargetSandboxVersion
		{
			get
			{
				List<String> targetSandboxVersion = base.Node.GetAttribute("targetSandboxVersion");
				return targetSandboxVersion == null
					? 1
					: Convert.ToInt32(targetSandboxVersion[0]);
			}
		}

		/// <summary>A user-readable label for the shared user ID.</summary>
		/// <remarks>The label must be set as a reference to a string resource; it cannot be a raw string.</remarks>
		[Obsolete("This constant was deprecated in API level 29", false)]
		public String SharedUserLabel
		{
			get
			{
				List<String> sharedUserLabel = base.Node.GetAttribute("sharedUserLabel");
				return sharedUserLabel == null
					? null
					: base.GetResource(Convert.ToInt32(sharedUserLabel[0])).Value;
			}
		}

		/// <summary>
		/// An internal version number.
		/// This number is used only to determine whether one version is more recent than another, with higher numbers indicating more recent versions.
		/// This is not the version number shown to users; that number is set by the versionName attribute.
		/// </summary>
		/// <remarks>
		/// The value must be set as an integer, such as "100".
		/// You can define it however you want, as long as each successive version has a higher number.
		/// For example, it could be a build number.
		/// Or you could translate a version number in "x.y" format to an integer by encoding the "x" and "y" separately in the lower and upper 16 bits.
		/// Or you could simply increase the number by one each time a new version is released.
		/// </remarks>
		public String VersionCode
		{
			get
			{
				List<String> versionCode = base.Node.GetAttribute("versionCode");
				return versionCode == null
					? null
					: versionCode[0];
			}
		}

		/// <summary>The version number shown to users</summary>
		/// <remarks>
		/// This attribute can be set as a raw string or as a reference to a string resource.
		/// The string has no other purpose than to be displayed to users.
		/// The versionCode attribute holds the significant version number used internally.
		/// </remarks>
		public String VersionName
		{
			get
			{
				List<String> result = base.Node.GetAttribute("versionName");
				if(result == null)
					return null;

				Int32 resourceId;
				if(Int32.TryParse(result[0], out resourceId))
				{
					ResourceRow resource = base.GetResource(resourceId);
					return resource == null
						? null
						: resource.Value;
				} else
					return result[0];
			}
		}

		/// <summary>The default install location for the app</summary>
		/// <remarks>
		/// When an app is installed on the external storage:
		/// The .apk file is saved to the external storage, but any app data (such as databases) is still saved on the internal device memory.
		/// 
		/// The container in which the .apk file is saved is encrypted with a key that allows the app to operate only on the device that installed it.
		/// (A user cannot transfer the SD card to another device and use apps installed on the card.)
		/// Though, multiple SD cards can be used with the same device.
		/// 
		/// At the user's request, the app can be moved to the internal storage.
		/// 
		/// The user may also request to move an app from the internal storage to the external storage.
		/// However, the system will not allow the user to move the app to external storage if this attribute is set to internalOnly, which is the default setting.
		/// </remarks>
		public InstallLocationType InstallLocation
		{
			get
			{
				List<String> installLocation = base.Node.GetAttribute("installLocation");
				return installLocation == null
					? InstallLocationType.internalOnly
					: (InstallLocationType)Enum.Parse(typeof(InstallLocationType), installLocation[0]);
			}
		}

		/// <summary>The declaration of the application</summary>
		public ApkApplication Application { get { return new ApkApplication(this); } }

		/// <summary>Specifies a system permission that the user must grant in order for the app to operate correctly</summary>
		/// <remarks>
		/// Permissions are granted by the user when the application is installed (on devices running Android 5.1 and lower) or while the app is running (on devices running Android 6.0 and higher).
		/// </remarks>
		public IEnumerable<ApkUsesPermission> UsesPermission
		{
			get
			{
				foreach(XmlNode node in base.Node["uses-permission"])
					yield return new ApkUsesPermission(this, node);
			}
		}

		/// <summary>
		/// Specifies that an app wants a particular permission, but only if the app is installed on a device running Android 6.0 (API level 23) or higher.
		/// If the device is running API level 22 or lower, the app does not have the specified permission.
		/// </summary>
		/// <remarks>
		/// This element is useful when you update an app to include a new feature that requires an additional permission. If a user updates an app on a device that is running API level 22 or lower, the system prompts the user at install time to grant all new permissions that are declared in that update.
		/// If a new feature is minor enough, you may prefer to disable the feature altogether on those devices, so the user does not have to grant additional permissions to update the app.
		/// By using the <see cref="ApkUsesPermissionSdk23"/> element instead of <see cref="ApkUsesPermission"/>, you can request the permission only if the app is running on platforms that support the runtime permissions model, in which the user grants permissions to the app while it is running.
		/// </remarks>
		public IEnumerable<ApkUsesPermissionSdk23> UsesPermissionSdk23
		{
			get
			{
				foreach(XmlNode node in base.Node["uses-permission-sdk-23"])
					yield return new ApkUsesPermissionSdk23(this, node);
			}
		}

		/// <summary>Declares a single hardware or software feature that is used by the application</summary>
		/// <remarks>
		/// The purpose of a <see cref="ApkUsesFeature"/> declaration is to inform any external entity of the set of hardware and software features on which your application depends.
		/// The element offers a required attribute that lets you specify whether your application requires and cannot function without the declared feature, or whether it prefers to have the feature but can function without it.
		/// Because feature support can vary across Android devices, the <see cref="ApkUsesFeature"/> element serves an important role in letting an application describe the device-variable features that it uses.
		/// </remarks>
		public IEnumerable<ApkUsesFeature> UsesFeature
		{
			get
			{
				foreach(var node in base.Node["uses-feature"])
					yield return new ApkUsesFeature(this, node);
			}
		}

		/// <summary>Declares a security permission that can be used to limit access to specific components or features of this or other applications</summary>
		public IEnumerable<ApkPermission> Permission
		{
			get
			{
				foreach(var node in base.Node["permission"])
					yield return new ApkPermission(this, node);
			}
		}

		/// <summary>
		/// Declares a name for a logical grouping of related permissions.
		/// Individual permission join the group through the permissionGroup attribute of the <see cref="ApkPermission"/> element.
		/// Members of a group are presented together in the user interface.
		/// </summary>
		/// <remarks>
		/// Note that this element does not declare a permission itself, only a category in which permissions can be placed.
		/// See the <see cref="ApkPermission"/> element for element for information on declaring permissions and assigning them to groups.
		/// </remarks>
		public IEnumerable<ApkPermissionGroup> PermissionGroup
		{
			get
			{
				foreach(XmlNode node in base.Node["permission-group"])
					yield return new ApkPermissionGroup(this, node);
			}
		}

		/// <summary>Declares the base name for a tree of permissions</summary>
		/// <remarks>
		/// The application takes ownership of all names within the tree.
		/// It can dynamically add new permissions to the tree by calling PackageManager.addPermission().
		/// Names within the tree are separated by periods ('.').
		/// For example, if the base name is com.example.project.taxes, permissions like the following might be added:
		/// com.example.project.taxes.CALCULATE
		/// com.example.project.taxes.deductions.MAKE_SOME_UP
		/// com.example.project.taxes.deductions.EXAGGERATE
		/// Note that this element does not declare a permission itself, only a namespace in which further permissions can be placed.
		/// </remarks>
		/// <see cref="ApkPermission"/>
		public IEnumerable<ApkPermissionTree> PermissionTree
		{
			get
			{
				foreach(XmlNode node in base.Node["permission-tree"])
					yield return new ApkPermissionTree(this, node);
			}
		}

		/// <summary>Declares a single GL texture compression format that the app supports</summary>
		/// <remarks>
		/// An application "supports" a GL texture compression format if it is capable of providing texture assets that are compressed in that format, once the application is installed on a device.
		/// The application can provide the compressed assets locally, from inside the .apk, or it can download them from a server at runtime.
		/// </remarks>
		public IEnumerable<ApkSupportsGlTexture> SupportsGlTexture
		{
			get
			{
				foreach(XmlNode node in base.Node["supports-gl-texture"])
					yield return new ApkSupportsGlTexture(this, node);
			}
		}

		/// <summary>Lets you specify the screen sizes your application supports and enable screen compatibility mode for screens larger than what your application supports.</summary>
		/// <remarks>It's important that you always use this element in your application to specify the screen sizes your application supports.</remarks>
		public IEnumerable<ApkSupportsScreen> SupportsScreen
		{
			get
			{
				foreach(XmlNode node in base.Node["supports-screens"])
					yield return new ApkSupportsScreen(this, node);
			}
		}

		/// <summary>Indicates what hardware and software features the application requires</summary>
		/// <remarks>
		/// For example, an application might specify that it requires a physical keyboard or a particular navigation device, like a trackball.
		/// The specification is used to avoid installing the application on devices where it will not work.
		/// </remarks>
		public IEnumerable<ApkUsesConfiguration> UsesConfiguration
		{
			get
			{
				foreach(XmlNode node in base.Node["uses-configuration"])
					yield return new ApkUsesConfiguration(this, node);
			}
		}

		/// <summary>Create ApplicationManifest object</summary>
		/// <param name="node">ApplicationManifest.xml root node</param>
		/// <param name="resource">resource.arsc object</param>
		private AndroidManifest(XmlNode node, ArscFile resource)
			: base(node, resource.ResourceMap)
		{
		}

		/// <summary>Create instance AndroidManifest.xml</summary>
		/// <param name="axml">AXML file decoder</param>
		/// <param name="resources">ARSC file decoder</param>
		/// <returns>AndroidManifest object</returns>
		public static AndroidManifest Load(AxmlFile axml, ArscFile resources)
		{
			if(axml == null || resources == null)
				return null;

			if(!axml.Header.IsValid || !resources.Header.IsValid)
				return null;

			return new AndroidManifest(axml.RootNode, resources);
		}
	}
}