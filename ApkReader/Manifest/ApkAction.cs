using System;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Adds an action to an intent filter</summary>
	/// <remarks>
	/// An <see cref="ApkIntentFilter"/> element must contain one or more <see cref="ApkAction"/> elements.
	/// If there are no <see cref="ApkAction"/> elements in an intent filter, the filter doesn't accept any Intent objects.
	/// See Intents and Intent Filters for details on intent filters and the role of action specifications within a filter.
	/// </remarks>
	public class ApkAction : ApkNodeT<ApkIntentFilter>
	{
		/// <summary>The name of the action</summary>
		/// <remarks>
		///  Some standard actions are defined in the Intent class as ACTION_string constants.
		///  To assign one of these actions to this attribute, prepend "android.intent.action." to the string that follows ACTION_.
		///  For example, for ACTION_MAIN, use "android.intent.action.MAIN" and for ACTION_WEB_SEARCH, use "android.intent.action.WEB_SEARCH".
		/// </remarks>
		public String Name { get { return base.Node.Attributes["name"][0]; } }

		/// <summary>Описание возможного действия</summary>
		public String Description { get { return Resources.Intent.GetString(this.Name); } }

		internal ApkAction(ApkIntentFilter parentNode,XmlNode node)
			: base(parentNode, node)
		{
		}
	}
}