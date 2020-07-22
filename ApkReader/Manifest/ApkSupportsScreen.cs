using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Lets you specify the screen sizes your application supports and enable screen compatibility mode for screens larger than what your application supports.</summary>
	/// <remarks>It's important that you always use this element in your application to specify the screen sizes your application supports.</remarks>
	public class ApkSupportsScreen : ApkNodeT<AndroidManifest>
	{
		/// <summary>Indicates whether the application is resizeable for different screen sizes</summary>
		/// <remarks>
		/// This attribute is true, by default.
		/// If set false, the system will run your application in screen compatibility mode on large screens.
		/// </remarks>
		[DefaultValue(true)]
		[Obsolete("It was introduced to help applications transition from Android 1.5 to 1.6, when support for multiple screens was first introduced. You should not use it.", false)]
		public Boolean Resizeable
		{
			get { return base.GetBooleanAttribute("resizeable").GetValueOrDefault(true); }
		}

		/// <summary>Indicates whether the application supports smaller screen form-factors</summary>
		/// <remarks>
		/// A small screen is defined as one with a smaller aspect ratio than the "normal" (traditional HVGA) screen.
		/// An application that does not support small screens will not be available for small screen devices from external services (such as Google Play), because there is little the platform can do to make such an application work on a smaller screen.
		/// </remarks>
		[DefaultValue(true)]
		public Boolean SmallScreens
		{
			get { return base.GetBooleanAttribute("smallScreens").GetValueOrDefault(true); }
		}

		/// <summary>Indicates whether an application supports the "normal" screen form-factors</summary>
		/// <remarks> Traditionally this is an HVGA medium density screen, but WQVGA low density and WVGA high density are also considered to be normal</remarks>
		[DefaultValue(true)]
		public Boolean NormalScreens
		{
			get { return base.GetBooleanAttribute("normalScreens").GetValueOrDefault(true); }
		}

		/// <summary>Indicates whether the application supports larger screen form-factors</summary>
		/// <remarks>
		///  A large screen is defined as a screen that is significantly larger than a "normal" handset screen, and thus might require some special care on the application's part to make good use of it, though it may rely on resizing by the system to fill the screen.
		/// </remarks>
		public Boolean? LargeScreens
		{
			get { return base.GetBooleanAttribute("largeScreens"); }
		}

		/// <summary>Indicates whether the application supports extra large screen form-factors</summary>
		/// <remarks>
		/// An xlarge screen is defined as a screen that is significantly larger than a "large" screen, such as a tablet (or something larger) and may require special care on the application's part to make good use of it, though it may rely on resizing by the system to fill the screen.
		/// </remarks>
		public Boolean? XLargeScreens
		{
			get { return base.GetBooleanAttribute("xlargeScreens"); }
		}

		/// <summary>Indicates whether the application includes resources to accommodate any screen density</summary>
		public Boolean? AnyDensity
		{
			get { return base.GetBooleanAttribute("anyDensity"); }
		}

		/// <summary>Specifies the minimum smallestWidth required</summary>
		/// <remarks>
		/// The smallestWidth is the shortest dimension of the screen space (in dp units) that must be available to your application UI—that is, the shortest of the available screen's two dimensions.
		/// So, in order for a device to be considered compatible with your application, the device's smallestWidth must be equal to or greater than this value.
		/// (Usually, the value you supply for this is the "smallest width" that your layout supports, regardless of the screen's current orientation.)
		/// </remarks>
		public Int32? RequiresSmallestWidthDp
		{
			get
			{
				List<String> result = base.Node.GetAttribute("requiresSmallestWidthDp");
				return result == null
					? (Int32?)null
					: Convert.ToInt32(result[0]);
			}
		}

		/// <summary>
		/// This attribute allows you to enable screen compatibility mode as a user-optional feature by specifying the maximum "smallest screen width" for which your application is designed. If the smallest side of a device's available screen is greater than your value here, users can still install your application, but are offered to run it in screen compatibility mode.
		/// </summary>
		public Int32? CompatibleWidthLimitDp
		{
			get
			{
				List<String> result = base.Node.GetAttribute("compatibleWidthLimitDp");
				return result == null
					? (Int32?)null
					: Convert.ToInt32(result[0]);
			}
		}

		/// <summary>This attribute allows you to force-enable screen compatibility mode by specifying the maximum "smallest screen width" for which your application is designed</summary>
		/// <remarks>
		/// If your application is compatible with all screen sizes and its layout properly resizes, you do not need to use this attribute.
		/// Otherwise, you should first consider using the <see cref="CompatibleWidthLimitDp"/> attribute.
		/// You should use the <see cref="LargestWidthLimitDp"/> attribute only when your application is functionally broken when resized for larger screens and screen compatibility mode is the only way that users should use your application.
		/// </remarks>
		public Int32? LargestWidthLimitDp
		{
			get
			{
				List<String> result = base.Node.GetAttribute("largestWidthLimitDp");
				return result == null
					? (Int32?)null
					: Convert.ToInt32(result[0]);
			}
		}

		internal ApkSupportsScreen(AndroidManifest parentNode, XmlNode node)
			: base(parentNode,node)
		{

		}
	}
}