using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Declares a single hardware or software feature that is used by the application</summary>
	/// <remarks>
	/// The purpose of a <see cref="ApkUsesFeature"/> declaration is to inform any external entity of the set of hardware and software features on which your application depends.
	/// The element offers a required attribute that lets you specify whether your application requires and cannot function without the declared feature, or whether it prefers to have the feature but can function without it.
	/// Because feature support can vary across Android devices, the <see cref="ApkUsesFeature"/> element serves an important role in letting an application describe the device-variable features that it uses.
	/// </remarks>
	public class ApkUsesFeature : ApkNodeT<AndroidManifest>
	{
		/// <summary>Specifies a single hardware or software feature used by the application, as a descriptor string</summary>
		/// <remarks>
		/// Valid attribute values are listed in the Hardware features and Software features sections.
		/// These attribute values are case-sensitive.
		/// </remarks>
		public String Name
		{
			get
			{
				List<String> result = base.Node.GetAttribute("name");
				return result == null
					? null
					: result[0];
			}
		}

		/// <summary>Boolean value that indicates whether the application requires the feature specified in android:name</summary>
		/// <remarks>
		/// When you declare android:required="true" for a feature, you are specifying that the application cannot function, or is not designed to function, when the specified feature is not present on the device.
		/// When you declare android:required="false" for a feature, it means that the application prefers to use the feature if present on the device, but that it is designed to function without the specified feature, if necessary.
		/// </remarks>
		[DefaultValue(true)]
		public Boolean required
		{
			get { return base.GetBooleanAttribute("required").GetValueOrDefault(true); }
		}

		/// <summary>The OpenGL ES version required by the application</summary>
		/// <remarks>
		/// The higher 16 bits represent the major number and the lower 16 bits represent the minor number.
		/// For example, to specify OpenGL ES version 2.0, you would set the value as "0x00020000", or to specify OpenGL ES 3.2, you would set the value as "0x00030002".
		/// An application should specify at most one android:glEsVersion attribute in its manifest.
		/// If it specifies more than one, the android:glEsVersion with the numerically highest value is used and any other values are ignored.
		/// </remarks>
		public Int32? GlEsVersion
		{
			get
			{
				List<String> result = base.Node.GetAttribute("glEsVersion");
				return result == null
					? (Int32?)null
					: Convert.ToInt32(result[0]);
			}
		}

		/// <summary>Описание требуемой фичи</summary>
		public String Description
		{
			get
			{
				return this.Name == null
					? null
					: Resources.GetFeatures(this.Name);
			}
		}

		internal ApkUsesFeature(AndroidManifest parentNode, XmlNode usesFeatureNode)
			: base(parentNode, usesFeatureNode)
		{
		}
	}
}