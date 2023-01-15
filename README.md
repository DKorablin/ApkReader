## Apk reader

![Nuget](https://img.shields.io/nuget/v/AlphaOmega.ApkReader)

Android package file reader assembly. Can read apk files, android xml files (AndroidManifes.xml, *.xml), Dalvik executable format (dex), android resource files (arsc).

Usage:

    using(ApkFile apk = new ApkFile(filePath))
    {
        if(apk.IsValid)
        {
            If(apk.AndroidManifest!=null)
            {//AndroidManifest.xml
                Console.WriteLine("Package: {0}", apk.AndroidManifest.Package);
                Console.WriteLine("Application name: {0} ({1})", apk.AndroidManifest.Application.Label, apk.AndroidManifest.VersionName);
                //...
            }

            if(apk.Resources!=null)
            {//resources.arsc
                //...
            }

            foreach(String filePath in apk.GetHeaderFiles())
                if(Path.GetExtension(filePath).ToLowerInvariant()==".dex")
                    using(DexFile dex=new DexFile(new StreamLoader(apk.GetFileStream(filePath))))
                    {//Davlik executables
                        //...
                    }
        }
    }

Supported structures:
- ApkFile.cs
  - _XmlFile_ &mdash; AndroidManifest.xml
  - _Resources_ &mdash; resources.arsc
  - _AndroidManifest_ &mdash; Stronly typled android manifest file
- ArscFile.cs (resources.arsc)
  - _Header_ &mdash; Resource file header
  - _ResourceMap_ &mdash; Resource table (id,value(s))
- AxmlFile.cs (https://developer.android.com/guide/topics/manifest/manifest-intro)
  - Strongly typed manifest sections mapped to resources.arsc file where needed
- DexFile.cs (https://source.android.com/devices/tech/dalvik/dex-format)
  - _map_list_ &mdash; This is a list of the entire contents of a file, in order.
  - _STRING_ID_ITEM_ &mdash; String identifiers list.
  - _STRING_DATA_ITEM_ &mdash; String identifiers list.
  - _CODE_ITEM_ &mdash; Source bytecode payload
  - _TYPE_ID_ITEM_ &mdash; Type identifiers list.
  - _TYPE_LIST_ &mdash; Referenced from Class definitions list and method prototype identifiers list.
  - _PROTO_ID_ITEM_ &mdash; Method prototype identifiers list
  - _FIELD_ID_ITEM_ &mdash; Field identifiers list
  - _METHOD_ID_ITEM_ &mdash; Method identifiers list
  - _CLASS_DATA_ITEM_ &mdash; Class structure list
  - _encoded_field_ &mdash; Static and instance fields from the class_data_item
  - _encoded_method_ &mdash; Class method definition list
  - _CLASS_DEF_ITEM_ &mdash; Class definitions list
  - _try_item_ &mdash; in the code exceptions are caught and how to handle them
  - _encoded_catch_handler_list_ &mdash; Catch handler lists
  - _encoded_type_addr_pair_ &mdash; One for each caught type, in the order that the types should be tested