using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("ad049202-57a1-4f5b-aef3-88551b6e09b2")]
[assembly: System.CLSCompliant(false)]

#if NETSTANDARD
[assembly: AssemblyMetadata("RepositoryUrl", "https://github.com/DKorablin/ApkReader")]
#else
[assembly: AssemblyTitle("APK Reader")]
[assembly: AssemblyProduct("Android Package Reader")]
[assembly: AssemblyCompany("Danila Korablin")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2016-2023")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
//[assembly: AssemblyVersion("1.0.0.0")]
#endif