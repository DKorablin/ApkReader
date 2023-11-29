using System;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Declares a single GL texture compression format that the app supports</summary>
	/// <remarks>
	/// An application "supports" a GL texture compression format if it is capable of providing texture assets that are compressed in that format, once the application is installed on a device.
	/// The application can provide the compressed assets locally, from inside the .apk, or it can download them from a server at runtime.
	/// </remarks>
	public class ApkSupportsGlTexture : ApkNodeT<AndroidManifest>
	{
		/// <summary>Specifies a single GL texture compression format supported by the application, as a descriptor string</summary>
		public String Name => base.Node.Attributes["name"][0];

		internal ApkSupportsGlTexture(AndroidManifest manifest, XmlNode node)
			: base(manifest, node)
		{ }
	}
}