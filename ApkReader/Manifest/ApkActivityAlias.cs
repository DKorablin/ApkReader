using System;
using System.Collections.Generic;
using System.ComponentModel;
using AlphaOmega.Debug.Arsc;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>
	/// The alias presents the target activity as an independent entity.
	/// It can have its own set of intent filters, and they, rather than the intent filters on the target activity itself, determine which intents can activate the target through the alias and how the system treats the alias.
	/// For example, the intent filters on the alias may specify the "android.intent.action.MAIN" and "android.intent.category.LAUNCHER" flags, causing it to be represented in the application launcher, even though none of the filters on the target activity itself set these flags.
	/// </summary>
	/// <remarks>
	/// With the exception of targetActivity, <see cref="ApkActivityAlias"/> attributes are a subset of <see cref="ApkActivity"/> attributes.
	/// For attributes in the subset, none of the values set for the target carry over to the alias.
	/// However, for attributes not in the subset, the values set for the target activity also apply to the alias.
	/// </remarks>
	public class ApkActivityAlias : ApkNodeT<ApkApplication>, IApkIntentedNode
	{
		/// <summary>Whether or not the target activity can be instantiated by the system through this alias — "true" if it can be, and "false" if not.</summary>
		/// <remarks>
		/// The <see cref="ApkApplication"/> element has its own enabled attribute that applies to all application components, including activity aliases.
		/// The <see cref="ApkApplication"/> and <see cref="ApkActivityAlias"/> attributes must both be "true" for the system to be able to instantiate the target activity through the alias.
		/// If either is "false", the alias does not work.
		/// </remarks>
		[DefaultValue(true)]
		public Boolean Enabled
			=> base.GetBooleanAttribute("enabled").GetValueOrDefault(true);

		/// <summary>
		/// Whether or not components of other applications can launch the target activity through this alias — "true" if they can, and "false" if not.
		/// If "false", the target activity can be launched through the alias only by components of the same application as the alias or applications with the same user ID.
		/// </summary>
		public Boolean Exported
		{
			get
			{
				Boolean? result = base.GetBooleanAttribute("exported");
				return result == null
					? base.Node.ChildNodes.ContainsKey("intent-filter")
					: result.Value;
			}
		}

		/// <summary>An icon for the target activity when presented to users through the alias</summary>
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
		/// A user-readable label for the activity.
		/// The label is displayed on-screen when the activity must be represented to the user.
		/// It's often displayed along with the activity icon.
		/// </summary>
		/// <see cref="ApkApplication.Label"/>
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

		/// <summary>A unique name for the alias</summary>
		/// <remarks>
		/// The name should resemble a fully qualified class name.
		/// But, unlike the name of the target activity, the alias name is arbitrary; it does not refer to an actual class.
		/// </remarks>
		public String Name
			=> base.Node.Attributes["name"][0];

		/// <summary>
		/// The name of a permission that clients must have to launch the target activity or get it to do something via the alias. If a caller of startActivity() or startActivityForResult() has not been granted the specified permission, the target activity will not be activated. 
		/// </summary>
		/// <remarks>This attribute supplants any permission set for the target activity itself. If it is not set, a permission is not needed to activate the target through the alias.</remarks>
		public String Permission
		{
			get
			{
				List<String> result = base.Node.GetAttribute("permission");
				return result?[0];
			}
		}

		/// <summary>The name of the activity that can be activated through the alias</summary>
		/// <remarks>This name must match the name attribute of an <see cref="ApkActivity"/> element that precedes the alias in the manifest.</remarks>
		public String TargetActivity
		{
			get
			{
				List<String> result = base.Node.GetAttribute("targetActivity");
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

		internal ApkActivityAlias(ApkApplication parentNode, XmlNode node)
			: base(parentNode, node)
		{ }
	}
}