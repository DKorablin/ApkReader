using System;
using System.Collections.Generic;
using System.ComponentModel;
using AlphaOmega.Debug.Arsc;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Declares a security permission that can be used to limit access to specific components or features of this or other applications</summary>
	public class ApkPermission : ApkNodeT<AndroidManifest>
	{
		/// <summary>Characterizes the potential risk implied in the permission and indicates the procedure the system should follow when determining whether or not to grant the permission to an application requesting it</summary>
		[Flags]
		public enum ProtectionLevelType
		{
			/// <summary>
			/// A lower-risk permission that gives requesting applications access to isolated application-level features, with minimal risk to other applications, the system, or the user.
			/// The system automatically grants this type of permission to a requesting application at installation, without asking for the user's explicit approval (though the user always has the option to review these permissions before installing).
			/// </summary>
			normal,
			/// <summary>
			/// A higher-risk permission that would give a requesting application access to private user data or control over the device that can negatively impact the user.
			/// Because this type of permission introduces potential risk, the system may not automatically grant it to the requesting application.
			/// For example, any dangerous permissions requested by an application may be displayed to the user and require confirmation before proceeding, or some other approach may be taken to avoid the user automatically allowing the use of such facilities.
			/// </summary>
			dangerous,
			/// <summary>
			/// A permission that the system grants only if the requesting application is signed with the same certificate as the application that declared the permission.
			/// If the certificates match, the system automatically grants the permission without notifying the user or asking for the user's explicit approval.
			/// </summary>
			signature,
			/// <summary>
			/// A permission that the system grants only to applications that are in a dedicated folder on the Android system image or that are signed with the same certificate as the application that declared the permission.
			/// Avoid using this option, as the signature protection level should be sufficient for most needs and works regardless of exactly where apps are installed.
			/// The "signatureOrSystem" permission is used for certain special situations where multiple vendors have applications built into a system image and need to share specific features explicitly because they are being built together.
			/// </summary>
			/// <remarks>Old synonym for "signature|privileged". Deprecated in API level 23.</remarks>
			signatureOrSystem,
		}

		/// <summary>A user-readable description of the permission, longer and more informative than the label</summary>
		/// <remarks> It may be displayed to explain the permission to the user — for example, when the user is asked whether to grant the permission to another application.</remarks>
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

		/// <summary>A reference to a drawable resource for an icon that represents the permission</summary>
		public String Icon
		{
			get
			{
				List<String> result = base.Node.GetAttribute("icon");
				return result == null
					? null
					: base.GetResource(Convert.ToInt32(result[0])).Value;
			}
		}

		/// <summary>A name for the permission, one that can be displayed to users</summary>
		public String Label
		{
			get
			{
				List<String> result = base.Node.GetAttribute("label");
				if(result == null)
					return null;
				else
				{
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
		}

		/// <summary>The name of the permission</summary>
		/// <remarks>
		///  This is the name that will be used in code to refer to the permission — for example, in a <see cref="ApkUsesPermission"/> element and the permission attributes of application components
		/// </remarks>
		public String Name
		{
			get { return base.Node.Attributes["name"][0]; }
		}

		/// <summary>Assigns this permission to a group</summary>
		/// <remarks>
		///  The value of this attribute is the name of the group, which must be declared with the <see cref="ApkPermissionGroup"/> element in this or another application.
		/// </remarks>
		public String PermissionGroup
		{
			get
			{
				List<String> result = base.Node.GetAttribute("permissionGroup");
				return result == null
					? null
					: result[0];
			}
		}

		/// <summary>Characterizes the potential risk implied in the permission and indicates the procedure the system should follow when determining whether or not to grant the permission to an application requesting it</summary>
		[DefaultValue(ProtectionLevelType.normal)]
		public ProtectionLevelType ProtectionLevel
		{
			get
			{
				List<String> result = base.Node.GetAttribute("protectionLevel");
				return result == null
					? ProtectionLevelType.normal
					: (ProtectionLevelType)Enum.Parse(typeof(ProtectionLevelType), result[0]);
			}
		}

		internal ApkPermission(AndroidManifest parentNode, XmlNode node)
			: base(parentNode, node)
		{

		}
	}
}