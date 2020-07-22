using System;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Adds a category name to an intent filter</summary>
	/// <remarks> See Intents and Intent Filters for details on intent filters and the role of category specifications within a filter.</remarks>
	public class ApkCategory : ApkNodeT<ApkIntentFilter>
	{
		/// <summary>The name of the category</summary>
		/// <remarks>
		/// Standard categories are defined in the Intent class as CATEGORY_name constants.
		/// The name assigned here can be derived from those constants by prefixing "android.intent.category." to the name that follows CATEGORY_.
		/// For example, the string value for CATEGORY_LAUNCHER is "android.intent.category.LAUNCHER".
		/// </remarks>
		public String Name
		{
			get { return base.Node.Attributes["name"][0]; }
		}

		internal ApkCategory(ApkIntentFilter parentNode, XmlNode node)
			: base(parentNode, node)
		{

		}
	}
}