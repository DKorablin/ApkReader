Apk reader
===============

Android package file reader assembly. Can read apk files (http://icsharpcode.github.io/SharpZipLib/), android xml files (AndroidManifes.xml, *.xml), Dalvik executable format (dex), android resource files (arsc).

Usage:
<pre>
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
</pre>

Supported structures:
<ul>
	<li>ApkFile.cs
		<ul>
			<li><i>XmlFile</i> &mdash; AndroidManifest.xml</li>
			<li><i>Resources</i> &mdash; resources.arsc</li>
			<li><i>AndroidManifest</i> &mdash; Stronly typled android manifest file</li>
		</ul>
	</li>
	<li>ArscFile.cs (resources.arsc)
		<ul>
			<li><i>Header</i> &mdash; Resource file header</li>
			<li><i>ResourceMap</i> &mdash; Resource table (id,value(s))</li>
		</ul>
	</li>
	<li>AxmlFile.cs (https://developer.android.com/guide/topics/manifest/manifest-intro)
		<ul>
			<li>Strongly typed manifest sections mapped to resources.arsc file where needed</li>
		</ul>
	</li>
	<li>DexFile.cs (https://source.android.com/devices/tech/dalvik/dex-format)
		<ul>
			<li><i>map_list</i> &mdash; This is a list of the entire contents of a file, in order.</li>
			<li><i>STRING_ID_ITEM</i> &mdash; String identifiers list.</li>
			<li><i>STRING_DATA_ITEM</i> &mdash; String identifiers list.</li>
			<li><i>CODE_ITEM</i> &mdash; Source bytecode payload</li>
			<li><i>TYPE_ID_ITEM</i> &mdash; Type identifiers list.</li>
			<li><i>TYPE_LIST</i> &mdash; Referenced from Class definitions list and method prototype identifiers list.</li>
			<li><i>PROTO_ID_ITEM</i> &mdash; Method prototype identifiers list</li>
			<li><i>FIELD_ID_ITEM</i> &mdash; Field identifiers list</li>
			<li><i>METHOD_ID_ITEM</i> &mdash; Method identifiers list</li>
			<li><i>CLASS_DATA_ITEM</i> &mdash; Class structure list</li>
			<li><i>encoded_field</i> &mdash; Static and instance fields from the class_data_item</li>
			<li><i>encoded_method</i> &mdash; Class method definition list</li>
			<li><i>CLASS_DEF_ITEM</i> &mdash; Class definitions list</li>
			<li><i>try_item</i> &mdash; in the code exceptions are caught and how to handle them</li>
			<li><i>encoded_catch_handler_list</i> &mdash; Catch handler lists</li>
			<li><i>encoded_type_addr_pair</i> &mdash; One for each caught type, in the order that the types should be tested</li>
		</ul>
	</li>
</ul>
