using System;
using System.Collections.Generic;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Specifies a system permission that the user must grant in order for the app to operate correctly</summary>
	/// <remarks>
	/// Permissions are granted by the user when the application is installed (on devices running Android 5.1 and lower) or while the app is running (on devices running Android 6.0 and higher).
	/// </remarks>
	public class ApkUsesPermission : ApkNodeT<AndroidManifest>
	{
		/// <summary>The name of the permission</summary>
		/// <remarks>
		/// It can be a permission defined by the application with the <see cref="ApkPermission"/> element, a permission defined by another application,
		/// or one of the standard system permissions (such as "android.permission.CAMERA" or "android.permission.READ_CONTACTS").
		/// As these examples show, a permission name typically includes the package name as a prefix.
		/// </remarks>
		public String Name { get { return base.Node.Attributes["name"][0]; } }

		/// <summary>The highest API level at which this permission should be granted to your app</summary>
		/// <remarks>
		/// Setting this attribute is useful if the permission your app requires is no longer needed beginning at a certain API level.
		/// For example, beginning with Android 4.4 (API level 19), it's no longer necessary for your app to request the WRITE_EXTERNAL_STORAGE permission
		/// when your app wants to write to its own application-specific directories on external storage (the directories provided by getExternalFilesDir()).
		/// However, the permission is required for API level 18 and lower. So you can declare that this permission is needed only up to API level 18.
		/// </remarks>
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
		public String Description { get { return Resources.Permission.GetString(this.Name); } }

		internal ApkUsesPermission(AndroidManifest parentNode, XmlNode usesPermissionNode)
			: base(parentNode, usesPermissionNode)
		{
		}
	}
}