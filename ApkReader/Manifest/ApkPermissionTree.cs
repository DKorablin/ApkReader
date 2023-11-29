using System;
using System.Collections.Generic;
using AlphaOmega.Debug.Arsc;

namespace AlphaOmega.Debug.Manifest
{
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
	public class ApkPermissionTree : ApkNodeT<AndroidManifest>
	{
		/// <summary>An icon representing all the permissions in the tree</summary>
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

		/// <summary>A user-readable name for the group</summary>
		public String Label
		{
			get
			{
				List<String> result = base.Node.GetAttribute("label");
				if(result == null)
					return null;
				else
				{
					if(Int32.TryParse(result[0], out Int32 resourceId))
					{
						ResourceRow resource = base.GetResource(resourceId);
						if(resource != null)
							return resource.Value;
					}
					return result[0];
				}
			}
		}

		/// <summary>The name that's at the base of the permission tree</summary>
		/// <remarks>
		/// It serves as a prefix to all permission names in the tree.
		/// Java-style scoping should be used to ensure that the name is unique.
		/// The name must have more than two period-separated segments in its path — for example, com.example.base is OK, but com.example is not.
		/// </remarks>
		public String Name => base.Node.Attributes["name"][0];

		internal ApkPermissionTree(AndroidManifest parentNode, XmlNode node)
			: base(parentNode, node)
		{ }
	}
}