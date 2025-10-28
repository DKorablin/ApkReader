# Apk Reader
[![Auto build](https://github.com/DKorablin/ApkReader/actions/workflows/release.yml/badge.svg)](https://github.com/DKorablin/ApkReader/actions)
[![NuGet](https://img.shields.io/nuget/v/AlphaOmega.ApkReader)](https://www.nuget.org/packages/AlphaOmega.ApkReader)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AlphaOmega.ApkReader)](https://www.nuget.org/packages/AlphaOmega.ApkReader)

Android APK analysis library. Reads:
- AndroidManifest.xml (binary AXML -> strongly typed model)
- resources.arsc (resource table)
- classes*.dex (Dalvik/DEX structures)
- JAR V1 (MANIFEST.MF / *.SF) & APK V2+ signature block metadata
- Generic files inside the ZIP/APK container

Target frameworks: .NET Framework 2.0, .NET Standard 2.0, .NET 8

## Installation
NuGet:
```powershell
Install-Package AlphaOmega.ApkReader
# or
dotnet add package AlphaOmega.ApkReader
```

## Quick usage
```csharp
using System;
using System.IO;
using AlphaOmega.Debug;
using AlphaOmega.Debug.Dex; // If you process dex

String apkPath = "app.apk";
using(ApkFile apk = new ApkFile(apkPath))
{
    if(!apk.IsValid)
    {
        Console.WriteLine("Invalid APK structure");
        return;
    }

    // Manifest (strongly typed)
    if(apk.AndroidManifest != null)
    {
        Console.WriteLine("Package: {0}", apk.AndroidManifest.Package);
        Console.WriteLine("Version: {0} ({1})", apk.AndroidManifest.VersionName, apk.AndroidManifest.VersionCode);
        Console.WriteLine("App Label: {0}", apk.AndroidManifest.Application.Label);
    }

    // Permissions
    foreach(String perm in apk.UsesPermission)
        Console.WriteLine("Permission: {0}", perm);

    // Resources
    if(apk.Resources != null)
        Console.WriteLine("Resource entries: {0}", apk.Resources.ResourceMap.Count);

    // Enumerate files
    foreach(String filePathInApk in apk.EnumerateFiles())
    {
        if(Path.GetExtension(filePathInApk).Equals(".dex", StringComparison.OrdinalIgnoreCase))
        {
            using(Stream stream = apk.GetFile(filePathInApk, checkSignature:true)?.Let(b => new MemoryStream(b)))
            {
                if(stream != null)
                using(DexFile dex = new DexFile(new StreamLoader(stream)))
                {
                    Console.WriteLine("Dex: strings={0} types={1} methods={2}",
                        dex.StringIdItems.Length,
                        dex.TypeIdItems.Length,
                        dex.MethodIdItems.Length);
                }
            }
        }
    }
}

// Helper extension (optional)
static class Extensions
{
    public static T Let<TIn,T>(this TIn value, Func<TIn,T> selector) => selector(value);
}
```

## Main types
- `ApkFile` – entry point for reading APK internals
- `AxmlFile` – raw binary Android XML file parser
- `AndroidManifest` – strongly typed manifest (sections map to resources where needed)
- `ArscFile` – resources.arsc reader (header + resource table)
- `DexFile` – Dalvik/DEX format reader (map list, string/type/proto/field/method/class structures, code items, try/catch handlers)
- `ApkSignature` – APK signatures (V1 JAR manifest + V2+ signature block). Extracts issuer certificate (V2) where available
- `MfFile` – JAR file integrity validation (MANIFEST.MF)

## Signatures & Validation
- V1 (JAR): validates file hashes via MANIFEST.MF
- V2+: reads APK Signature Block (see Android docs)
- Use `apk.Signatures` to access schemes. Example:
```csharp
var sig = apk.Signatures;
bool hasV1 = sig.V1SchemeBlock.Manifest != null;
var certs = sig.V2SchemeBlock?.SignerCertificates; // May be null
```

## Getting file bytes
```csharp
byte[] manifestBytes = apk.GetFile("AndroidManifest.xml", checkSignature:true);
byte[] dexBytes = apk.GetFile("classes.dex");
```
`checkSignature:true` performs hash validation against V1 manifest when possible.

## Supported structures (summary)
- ApkFile
  - XmlFile – AndroidManifest.xml (raw AXML)
  - Resources – resources.arsc
  - AndroidManifest – typed manifest
- ApkSignature
  - Signature block, V1 / V2 metadata, issuer certificate extraction (needs broader testing)
- ArscFile
  - Header
  - ResourceMap (id, value(s))
- MfFile
  - JAR integrity (MANIFEST.MF / *.SF)
- AxmlFile
  - Typed sections mapped to resources
- DexFile (Dalvik format)
  - map_list, STRING_ID_ITEM / STRING_DATA_ITEM, TYPE_ID_ITEM, TYPE_LIST
  - PROTO_ID_ITEM, FIELD_ID_ITEM, METHOD_ID_ITEM
  - CLASS_DEF_ITEM + CLASS_DATA_ITEM (encoded_field / encoded_method)
  - CODE_ITEM (bytecode payload)
  - try_item, encoded_catch_handler_list, encoded_type_addr_pair

## Performance notes
Lazy loading: manifest, resources and signature blocks load only when first accessed.
Large files: prefer stream constructor to avoid copying bytes.

## Limitations / TODO
- More exhaustive V2/V3 signature validation
- Resource value convenience helpers
- Multi-dex convenience enumeration
- Unit tests coverage expansion
- .NET Framework 2.0 version contains the reference to SharpZipLib 0.86.0 with vulnerabilities. Upgrade when possible.

## Development
Clone and build:
```bash
git clone https://github.com/DKorablin/ApkReader.git
cd ApkReader
# Build (uses multi-target project)
dotnet build
```