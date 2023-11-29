using System;
using System.Collections.Generic;
using AlphaOmega.Debug.Arsc;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>
	/// Declares a name for a logical grouping of related permissions.
	/// Individual permission join the group through the permissionGroup attribute of the <see cref="ApkPermission"/> element.
	/// Members of a group are presented together in the user interface.
	/// </summary>
	/// <remarks>
	/// Note that this element does not declare a permission itself, only a category in which permissions can be placed.
	/// See the <see cref="ApkPermission"/> element for element for information on declaring permissions and assigning them to groups.
	/// </remarks>
	public class ApkPermissionGroup : ApkNodeT<AndroidManifest>
	{
		/// <summary>User-readable text that describes the group</summary>
		/// <remarks>The text should be longer and more explanatory than the label</remarks>
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

		/// <summary>An icon representing the permission</summary>
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

		/// <summary>The name of the group</summary>
		/// <remarks>This is the name that can be assigned to a <see cref="ApkPermission"/> element's <see cref="ApkPermissionGroup"/> attribute.</remarks>
		public String Name
		{
			get
			{
				List<String> result = base.Node.GetAttribute("name");
				return result?[0];
			}
		}

		internal ApkPermissionGroup(AndroidManifest parentNode, XmlNode node)
			: base(parentNode, node)
		{ }
	}
}