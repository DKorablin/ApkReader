using System;
using System.Collections.Generic;
using System.ComponentModel;
using AlphaOmega.Debug.Arsc;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Specifies the types of intents that an activity, service, or broadcast receiver can respond to</summary>
	/// <remarks>
	///  An intent filter declares the capabilities of its parent component — what an activity or service can do and what types of broadcasts a receiver can handle.
	///  It opens the component to receiving intents of the advertised type, while filtering out those that are not meaningful for the component.
	/// </remarks>
	public class ApkIntentFilter : ApkNode
	{
		private readonly IApkIntentedNode _parentNode;

		/// <summary>An icon that represents the parent activity, service, or broadcast receiver when that component is presented to the user as having the capability described by the filter</summary>
		public String Icon
		{
			get
			{
				List<String> result = base.Node.GetAttribute("icon");
				return result == null
					? this._parentNode.Icon
					: base.GetResource(Convert.ToInt32(result[0])).Value;
			}
		}

		/// <summary>A user-readable label for the parent component</summary>
		/// <remarks>This label, rather than the one set by the parent component, is used when the component is presented to the user as having the capability described by the filter.</remarks>
		public String Label
		{
			get
			{
				List<String> result = base.Node.GetAttribute("label");
				if(result == null)
					return this._parentNode.Label;
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

		/// <summary>The priority that should be given to the parent component with regard to handling intents of the type described by the filter</summary>
		/// <remarks>
		/// This attribute has meaning for both activities and broadcast receivers:
		/// 
		/// It provides information about how able an activity is to respond to an intent that matches the filter, relative to other activities that could also respond to the intent.
		/// When an intent could be handled by multiple activities with different priorities, Android will consider only those with higher priority values as potential targets for the intent.
		/// 
		/// It controls the order in which broadcast receivers are executed to receive broadcast messages.
		/// Those with higher priority values are called before those with lower values. (The order applies only to synchronous messages; it's ignored for asynchronous messages.)
		/// 
		/// Use this attribute only if you really need to impose a specific order in which the broadcasts are received, or want to force Android to prefer one activity over others.
		/// </remarks>
		[DefaultValue(0)]
		public Int32 Priority
		{
			get
			{
				List<String> result = base.Node.GetAttribute("priority");
				return result == null
					? 0
					: Convert.ToInt32(result[0]);
			}
		}

		/// <summary>The order in which the filter should be processed when multiple filters match.</summary>
		/// <remarks>
		/// Order differs from priority in that priority applies across apps, while order disambiguates multiple matching filters in a single app.
		/// When multiple filters could match, use a directed intent instead.
		/// </remarks>
		[DefaultValue(0)]
		public Int32 Order
		{
			get
			{
				List<String> result = base.Node.GetAttribute("order");
				return result == null
					? 0
					: Convert.ToInt32(result[0]);
			}
		}

		/// <summary>Adds an action to an intent filter</summary>
		/// <remarks>
		/// An <see cref="ApkIntentFilter"/> element must contain one or more <see cref="ApkAction"/> elements.
		/// If there are no <see cref="ApkAction"/> elements in an intent filter, the filter doesn't accept any Intent objects.
		/// See Intents and Intent Filters for details on intent filters and the role of action specifications within a filter.
		/// </remarks>
		public IEnumerable<ApkAction> Action
		{
			get
			{
				foreach(XmlNode node in base.Node.ChildNodes["action"])
					yield return new ApkAction(this, node);
			}
		}

		/// <summary>Adds a category name to an intent filter</summary>
		/// <remarks> See Intents and Intent Filters for details on intent filters and the role of category specifications within a filter.</remarks>
		public IEnumerable<ApkCategory> Category
		{
			get
			{
				foreach(XmlNode node in base.Node["category"])
					yield return new ApkCategory(this, node);
			}
		}

		/// <summary>Adds a data specification to an intent filter</summary>
		/// <remarks>
		/// The specification can be just a data type (the mimeType attribute), just a URI, or both a data type and a URI.
		/// A URI is specified by separate attributes for each of its parts:
		/// {scheme}://{host}:{port}[{path}|{pathPrefix}|{pathPattern}]
		/// </remarks>
		public IEnumerable<ApkData> Data
		{
			get
			{
				foreach(XmlNode node in base.Node["data"])
					yield return new ApkData(this, node);
			}
		}

		internal ApkIntentFilter(IApkIntentedNode parentNode, XmlNode node)
			: base((ApkNode)parentNode, node)
		{
			this._parentNode = parentNode;
		}
	}
}