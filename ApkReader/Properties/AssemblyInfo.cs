using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("ad049202-57a1-4f5b-aef3-88551b6e09b2")]

[assembly: System.CLSCompliant(false)]

#if !NETSTANDARD
[assembly: AssemblyCompany("Danila Korablin")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2016-2023")]
[assembly: AssemblyProduct("Android Package Reader")]
[assembly: AssemblyTitle("APK Reader")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
#endif