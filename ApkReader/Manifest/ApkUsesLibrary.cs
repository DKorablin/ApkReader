using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Specifies a shared library that the application must be linked against. This element tells the system to include the library's code in the class loader for the package.</summary>
	/// <remarks>
	/// All of the android packages (such as android.app, android.content, android.view, and android.widget) are in the default library that all applications are automatically linked against.
	/// However, some packages (such as maps) are in separate libraries that are not automatically linked. Consult the documentation for the packages you're using to determine which library contains the package code.
	/// </remarks>
	public class ApkUsesLibrary : ApkNodeT<ApkApplication>
	{
		/// <summary>The name of the library.</summary>
		/// <remarks>The name is provided by the documentation for the package you are using. An example of this is "android.test.runner", a package that contains Android test classes.</remarks>
		public String Name
		{
			get { return base.Node.Attributes["name"][0]; }
		}

		/// <summary>Boolean value that indicates whether the application requires the library specified by android:name</summary>
		/// <remarks>
		/// "true": The application does not function without this library. The system will not allow the application on a device that does not have the library.
		/// "false": The application can use the library if present, but is designed to function without it if necessary. The system will allow the application to be installed, even if the library is not present. If you use "false", you are responsible for checking at runtime that the library is available.
		/// </remarks>
		[DefaultValue(true)]
		public Boolean Required
		{
			get { return base.GetBooleanAttribute("required").GetValueOrDefault(true); }
		}

		internal ApkUsesLibrary(ApkApplication parentNode, XmlNode usesLibraryNode)
			: base(parentNode, usesLibraryNode)
		{

		}
	}
}