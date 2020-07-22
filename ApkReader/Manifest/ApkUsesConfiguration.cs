using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Indicates what hardware and software features the application requires</summary>
	/// <remarks>
	/// For example, an application might specify that it requires a physical keyboard or a particular navigation device, like a trackball.
	/// The specification is used to avoid installing the application on devices where it will not work.
	/// </remarks>
	public class ApkUsesConfiguration : ApkNodeT<AndroidManifest>
	{
		/// <summary>The type of keyboard the application requires, if any at all</summary>
		public enum KeyboardType
		{
			/// <summary>The application does not require a keyboard. (A keyboard requirement is not defined.)</summary>
			undefined,
			/// <summary>The application does not require a keyboard</summary>
			nokeys,
			/// <summary>The application requires a standard QWERTY keyboard</summary>
			qwerty,
			/// <summary>The application requires a twelve-key keypad, like those on most phones — with keys for the digits from 0 through 9 plus star (*) and pound (#) keys.</summary>
			twelvekey,
		}

		/// <summary>The navigation device required by the application, if any</summary>
		public enum NavigationType
		{
			/// <summary>The application does not require any type of navigation control. (The navigation requirement is not defined.) </summary>
			undefined,
			/// <summary>The application does not require a navigation control</summary>
			nonav,
			/// <summary>The application requires a D-pad (directional pad) for navigation</summary>
			dpad,
			/// <summary>The application requires a trackball for navigation</summary>
			trackball,
			/// <summary>The application requires a navigation wheel</summary>
			wheel,
		}

		/// <summary>The type of touch screen the application requires, if any at all</summary>
		public enum TouchScreenType
		{
			/// <summary>The application doesn't require a touch screen. (The touch screen requirement is undefined.)</summary>
			undefined,
			/// <summary>The application doesn't require a touch screen</summary>
			notouch,
			/// <summary>The application requires a touch screen that's operated with a stylus</summary>
			stylus,
			/// <summary>The application requires a touch screen that can be operated with a finger</summary>
			/// <remarks>If some type of touch input is required for your app, you should instead use the <see cref="ApkUsesFeature"/> tag to declare the required touchscreen type, beginning with "android.hardware.faketouch" for basic touch-style events.</remarks>
			finger,
		}

		/// <summary>Whether or not the application requires a five-way navigation control — "true" if it does, and "false" if not.</summary>
		/// <remarks>
		/// A five-way control is one that can move the selection up, down, right, or left, and also provides a way of invoking the current selection.
		/// It could be a D-pad (directional pad), trackball, or other device.
		/// </remarks>
		public Boolean? ReqFiveWayNav
		{
			get { return base.GetBooleanAttribute("reqFiveWayNav"); }
		}

		/// <summary>Whether or not the application requires a hardware keyboard — "true" if it does, and "false" if not.</summary>
		public Boolean? ReqHardKeyboard
		{
			get { return base.GetBooleanAttribute("reqHardKeyboard"); }
		}

		/// <summary>The type of keyboard the application requires, if any at all</summary>
		/// <remarks>
		/// This attribute does not distinguish between hardware and software keyboards.
		/// If a hardware keyboard of a certain type is required, specify the type here and also set the reqHardKeyboard attribute to "true".
		/// </remarks>
		[DefaultValue(KeyboardType.undefined)]
		public KeyboardType ReqKeyboardType
		{
			get
			{
				List<String> result = base.Node.GetAttribute("reqKeyboardType");
				return result == null
					? KeyboardType.undefined
					: (KeyboardType)Enum.Parse(typeof(KeyboardType), result[0]);
			}
		}

		/// <summary>The navigation device required by the application, if any</summary>
		/// <remarks>If an application requires a navigational control, but the exact type of control doesn't matter, it can set the reqFiveWayNav attribute to "true" rather than set this one.</remarks>
		[DefaultValue(NavigationType.undefined)]
		public NavigationType ReqNavigation
		{
			get
			{
				List<String> result = base.Node.GetAttribute("reqNavigation");
				return result == null
					? NavigationType.undefined
					: (NavigationType)Enum.Parse(typeof(NavigationType), result[0]);
			}
		}

		/// <summary>The type of touch screen the application requires, if any at all</summary>
		[DefaultValue(TouchScreenType.undefined)]
		public TouchScreenType ReqTouchScreen
		{
			get
			{
				List<String> result = base.Node.GetAttribute("reqTouchScreen");
				return result==null
					? TouchScreenType.undefined
					: (TouchScreenType)Enum.Parse(typeof(TouchScreenType), result[0]);
			}
		}


		internal ApkUsesConfiguration(AndroidManifest parentNode, XmlNode node)
			: base(parentNode, node)
		{

		}
	}
}