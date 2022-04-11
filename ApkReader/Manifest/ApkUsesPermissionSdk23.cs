using System;
using System.Collections.Generic;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>
	/// Specifies that an app wants a particular permission, but only if the app is installed on a device running Android 6.0 (API level 23) or higher.
	/// If the device is running API level 22 or lower, the app does not have the specified permission.
	/// </summary>
	/// <remarks>
	/// This element is useful when you update an app to include a new feature that requires an additional permission. If a user updates an app on a device that is running API level 22 or lower, the system prompts the user at install time to grant all new permissions that are declared in that update.
	/// If a new feature is minor enough, you may prefer to disable the feature altogether on those devices, so the user does not have to grant additional permissions to update the app.
	/// By using the <see cref="ApkUsesPermissionSdk23"/> element instead of <see cref="ApkUsesPermission"/>, you can request the permission only if the app is running on platforms that support the runtime permissions model, in which the user grants permissions to the app while it is running.
	/// </remarks>
	public class ApkUsesPermissionSdk23 : ApkNodeT<AndroidManifest>
	{
		/// <summary>The name of the permission</summary>
		/// <remarks>
		/// This permission can be defined by the app with the <see cref="ApkPermission"/>  element, it can be a permission defined by another app, or it can be one of the standard system permissions, such as "android.permission.CAMERA" or "android.permission.READ_CONTACTS".
		/// </remarks>
		public String Name { get { return base.Node.Attributes["name"][0]; } }

		/// <summary>The highest API level at which this permission should be granted to your app</summary>
		/// <remarks>If the app is installed on a device with a later API level, the app is not granted the permission and cannot use any related functionality.</remarks>
		public Int32? MaxSdkVersion
		{
			get
			{
				List<String> maxSdkVersion = base.Node.GetAttribute("maxSdkVersion");
				return maxSdkVersion == null
					? (Int32?)null
					: Convert.ToInt32(maxSdkVersion[0]);
			}
		}

		/// <summary>Описание системного разрешения</summary>
		public String Description { get { return Resources.GetPermission(this.Name); } }

		internal ApkUsesPermissionSdk23(AndroidManifest parentNode, XmlNode node)
			: base(parentNode,node)
		{
		}
	}
}