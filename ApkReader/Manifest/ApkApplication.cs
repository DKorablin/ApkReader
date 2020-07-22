using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using AlphaOmega.Debug.Arsc;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>The declaration of the application</summary>
	/// <remarks>
	/// This element contains subelements that declare each of the application's components and has attributes that can affect all the components.
	/// Many of these attributes (such as icon, label, permission, process, taskAffinity, and allowTaskReparenting) set default values for corresponding attributes of the component elements.
	/// Others (such as debuggable, enabled, description, and allowClearUserData) set values for the application as a whole and cannot be overridden by the components.
	/// </remarks>
	public class ApkApplication : ApkNodeT<AndroidManifest>
	{
		/// <summary>For more information about the app bar, see the Adding the App Bar training class</summary>
		public enum uiOptionsType
		{
			/// <summary>No extra UI options. This is the default</summary>
			none,

			/// <summary>
			/// Add a bar at the bottom of the screen to display action items in the app bar (also known as the action bar), when constrained for horizontal space (such as when in portrait mode on a handset).
			/// Instead of a small number of action items appearing in the app bar at the top of the screen, the app bar is split into the top navigation section and the bottom bar for action items.
			/// This ensures a reasonable amount of space is made available not only for the action items, but also for navigation and title elements at the top.
			/// Menu items are not split across the two bars; they always appear together.
			/// </summary>
			splitActionBarWhenNarrow,
		}

		/// <summary>
		/// Whether or not activities that the application defines can move from the task that started them to the task they have an affinity for when that task is next brought to the front — "true" if they can move, and "false" if they must remain with the task where they started.
		/// </summary>
		/// <remarks>
		/// The <see cref="ApkActivity"/> element has its own allowTaskReparenting attribute that can override the value set here.
		/// See that attribute for more information.
		/// </remarks>
		[DefaultValue(false)]
		public Boolean AllowTaskReparenting
		{
			get { return base.GetBooleanAttribute("allowTaskReparenting").GetValueOrDefault(false); }
		}

		/// <summary>Whether to allow the application to participate in the backup and restore infrastructure.</summary>
		/// <remarks>If this attribute is set to false, no backup or restore of the application will ever be performed, even by a full-system backup that would otherwise cause all application data to be saved via adb.</remarks>
		[DefaultValue(true)]
		public Boolean AllowBackup
		{
			get { return base.GetBooleanAttribute("allowBackup").GetValueOrDefault(true); }
		}

		/// <summary>Whether to allow the application to reset user data</summary>
		/// <remarks>This data includes flags—such as whether the user has seen introductory tooltips—as well as user-customizable settings and preferences.</remarks>
		[DefaultValue(true)]
		public Boolean AllowClearUserData
		{
			get { return base.GetBooleanAttribute("allowClearUserData").GetValueOrDefault(true); }
		}

		/// <summary>Whether to allow the application to reset user data</summary>
		/// <remarks>This data includes flags—such as whether the user has seen introductory tooltips—as well as user-customizable settings and preferences.</remarks>
		[DefaultValue(true)]
		public Boolean AllowNativeHeapPointerTagging
		{
			get { return base.GetBooleanAttribute("allowNativeHeapPointerTagging").GetValueOrDefault(true); }
		}

		/// <summary>The name of the class that implements the application's backup agent, a subclass of BackupAgent</summary>
		/// <remarks>
		/// The attribute value should be a fully qualified class name (such as, "com.example.project.MyBackupAgent").
		/// However, as a shorthand, if the first character of the name is a period (for example, ".MyBackupAgent"), it is appended to the package name specified in the <see cref="AndroidManifest"/> element.
		/// </remarks>
		public String BackupAgent
		{
			get
			{
				List<String> result = base.Node.GetAttribute("backupAgent");
				if(result == null)
					return null;
				else
					return result[0].StartsWith(".")
						? base.ParentNode.Package + result[0]
						: result[0];
			}
		}

		/// <summary>Indicates that Auto Backup operations may be performed on this app even if the app is in a foreground-equivalent state</summary>
		/// <remarks>The system shuts down an app during auto backup operation, so use this attribute with caution. Setting this flag to true can impact app behavior while the app is active</remarks>
		[DefaultValue(false)]
		public Boolean BackupInForeground
		{
			get { return base.GetBooleanAttribute("backupInForeground").GetValueOrDefault(false); }
		}

		/// <summary>
		/// A drawable resource providing an extended graphical banner for its associated item.
		/// Use with the <see cref="ApkApplication"/> tag to supply a default banner for all application activities, or with the <see cref="ApkActivity"/> tag to supply a banner for a specific activity.
		/// </summary>
		/// <remarks>
		/// The system uses the banner to represent an app in the Android TV home screen.
		/// Since the banner is displayed only in the home screen, it should only be specified by applications with an activity that handles the CATEGORY_LEANBACK_LAUNCHER intent.
		/// </remarks>
		public String Banner
		{
			get
			{
				List<String> result = base.Node.GetAttribute("banner");
				return result == null
					? null
					: base.GetResource(Convert.ToInt32(result[0])).Value;
			}
		}

		/// <summary>Whether or not the application can be debugged, even when running on a device in user mode — "true" if it can be, and "false" if not.</summary>
		[DefaultValue(false)]
		public Boolean Debuggable
		{
			get { return base.GetBooleanAttribute("debuggable").GetValueOrDefault(false); }
		}

		/// <summary>User-readable text about the application, longer and more descriptive than the application label</summary>
		/// <remarks>The value must be set as a reference to a string resource. Unlike the label, it cannot be a raw string</remarks>
		public String Description
		{
			get
			{
				List<String> result = base.Node.GetAttribute("description");
				return result == null
					? null
					: base.GetResource(Convert.ToInt32(result[0])).Value;
			}
		}

		/// <summary>Whether or not the application is direct-boot aware; that is, whether or not it can run before the user unlocks the device.</summary>
		/// <remarks>If you're using a custom subclass of Application, and if any component inside your application is direct-boot aware, then your entire custom application is considered to be direct-boot aware.</remarks>
		[DefaultValue(false)]
		public Boolean DirectBootAware
		{
			get { return base.GetBooleanAttribute("directBootAware").GetValueOrDefault(false); }
		}

		/// <summary>
		/// Whether or not the Android system can instantiate components of the application — "true" if it can, and "false" if not.
		/// If the value is "true", each component's enabled attribute determines whether that component is enabled or not.
		/// If the value is "false", it overrides the component-specific values; all components are disabled.
		/// </summary>
		[DefaultValue(true)]
		public Boolean Enabled
		{
			get { return base.GetBooleanAttribute("enabled").GetValueOrDefault(true); }
		}

		/// <summary>
		/// Whether or not the package installer extracts native libraries from the APK to the filesystem.
		/// If set to false, then your native libraries must be page aligned and stored uncompressed in the APK.
		/// No code changes are required as the linker loads the libraries directly from the APK at runtime.
		/// </summary>
		/// <remarks>
		/// The default value is "true".
		/// However, when building your app using Android Gradle plugin 3.6.0 or higher, this property is set to "false" by default.
		/// </remarks>
		[DefaultValue(true)]
		public Boolean ExtractNativeLibs
		{
			get { return base.GetBooleanAttribute("extractNativeLibs").GetValueOrDefault(true); }
		}

		/// <summary>
		/// This attribute points to an XML file that contains full backup rules for Auto Backup.
		/// These rules determine what files get backed up. For more information, see XML Config Syntax for Auto Backup.
		/// </summary>
		/// <remarks>
		/// This attribute is optional.
		/// If it is not specified, by default, Auto Backup includes most of your app's files. For more information, see Files that are backed up.
		/// </remarks>
		public String FullBackupContent
		{
			get
			{
				List<String> result = base.Node.GetAttribute("fullBackupContent");
				return result == null
					? null
					: result[0];
			}
		}

		/// <summary>This attribute indicates whether or not to use Auto Backup on devices where it is available.</summary>
		/// <remarks>
		/// If set to true, then your app performs Auto Backup when installed on a device running Android 6.0 (API level 23) or higher.
		/// On older devices, your app ignores this attribute and performs Key/Value Backups.
		/// </remarks>
		[DefaultValue(false)]
		public Boolean FullBackupOnly
		{
			get { return base.GetBooleanAttribute("fullBackupOnly").GetValueOrDefault(false); }
		}

		/// <summary>
		/// Whether or not the application contains any code — "true" if it does, and "false" if not.
		/// When the value is "false", the system does not try to load any application code when launching components. The default value is "true".
		/// </summary>
		/// <remarks>
		/// For example, if your app supports Google Play's Dynamic Delivery and includes dynamic feature modules that do not generate any DEX files—which is bytecode optimized for the Android platform—you need to set this property to false in the module's manifest file. Otherwise, you may get runtime errors.
		/// </remarks>
		[DefaultValue(true)]
		public Boolean HasCode
		{
			get { return base.GetBooleanAttribute("hasCode").GetValueOrDefault(true); }
		}

		/// <summary>When the user uninstalls an app, whether or not to show the user a prompt to keep the app's data</summary>
		[DefaultValue(false)]
		public Boolean HasFragileUserData
		{
			get { return base.GetBooleanAttribute("hasFragileUserData").GetValueOrDefault(false); }
		}

		/// <summary>Whether or not hardware-accelerated rendering should be enabled for all activities and views in this application — "true" if it should be enabled, and "false" if not.</summary>
		/// <remarks>
		/// Starting from Android 3.0 (API level 11), a hardware-accelerated OpenGL renderer is available to applications, to improve performance for many common 2D graphics operations.
		/// When the hardware-accelerated renderer is enabled, most operations in Canvas, Paint, Xfermode, ColorFilter, Shader, and Camera are accelerated.
		/// This results in smoother animations, smoother scrolling, and improved responsiveness overall, even for applications that do not explicitly make use the framework's OpenGL libraries.
		/// </remarks>
		[DefaultValue(true)]
		public Boolean HardwareAccelerated
		{
			get { return base.GetBooleanAttribute("hardwareAccelerated").GetValueOrDefault(true); }
		}

		/// <summary>An icon for the application as whole, and the default icon for each of the application's components</summary>
		/// <remarks>
		/// See the individual icon attributes for <see cref="ApkActivity"/>, <see cref="ApkActivityAlias"/>, <see cref="ApkService"/>, <see cref="ApkReceiver"/>, and <see cref="ApkProvider"/> elements.
		/// This attribute must be set as a reference to a drawable resource containing the image (for example "@drawable/icon"). There is no default icon.
		/// </remarks>
		public String Icon
		{
			get
			{
				List<String> result = base.Node.GetAttribute("icon");
				return result == null
					? null
					: base.GetResource(Convert.ToInt32(result[0])).Value;
			}
		}

		/// <summary>Whether or not the application is a game</summary>
		/// <remarks>The system may group together applications classifed as games or display them separately from other applications</remarks>
		[DefaultValue(false)]
		public Boolean IsGame
		{
			get { return base.GetBooleanAttribute("isGame").GetValueOrDefault(false); }
		}

		/// <summary>Whether the application in question should be terminated after its settings have been restored during a full-system restore operation</summary>
		/// <remarks>
		/// Single-package restore operations will never cause the application to be shut down.
		/// Full-system restore operations typically only occur once, when the phone is first set up.
		/// Third-party applications will not normally need to use this attribute.
		/// </remarks>
		[DefaultValue(true)]
		public Boolean KillAfterRestore
		{
			get { return base.GetBooleanAttribute("killAfterRestore").GetValueOrDefault(true); }
		}

		/// <summary>Whether your application's processes should be created with a large Dalvik heap.</summary>
		/// <remarks>
		/// This applies to all processes created for the application.
		/// It only applies to the first application loaded into a process;
		/// if you're using a shared user ID to allow multiple applications to use a process, they all must use this option consistently or they will have unpredictable results.
		/// 
		/// Most apps should not need this and should instead focus on reducing their overall memory usage for improved performance.
		/// Enabling this also does not guarantee a fixed increase in available memory, because some devices are constrained by their total available memory.
		/// </remarks>
		public Boolean? LargeHeap
		{
			get { return base.GetBooleanAttribute("largeHeap"); }
		}

		/// <summary>A user-readable label for the application as a whole, and a default label for each of the application's components.</summary>
		/// <remarks>
		/// See the individual label attributes for <see cref="ApkActivity"/>, <see cref="ApkActivityAlias"/>, <see cref="ApkService"/>, <see cref="ApkReceiver"/>, and <see cref="ApkProvider"/> elements.
		/// The label should be set as a reference to a string resource, so that it can be localized like other strings in the user interface.
		/// However, as a convenience while you're developing the application, it can also be set as a raw string.
		/// </remarks>
		public String Label
		{
			get
			{
				List<String> result = base.Node.GetAttribute("label");
				if(result == null)
					return null;

				Int32 resourceId;
				if(Int32.TryParse(result[0], out resourceId))
				{
					ResourceRow resource = base.GetResource(resourceId);
					return resource == null
						? null
						: resource.Value;
				} else
					return result[0];
			}
		}

		/// <summary>A logo for the application as whole, and the default logo for activities</summary>
		/// <remarks>This attribute must be set as a reference to a drawable resource containing the image (for example "@drawable/logo"). There is no default logo.</remarks>
		public String Logo
		{
			get
			{
				List<String> result = base.Node.GetAttribute("logo");
				return result == null
					? null
					: base.GetResource(Convert.ToInt32(result[0])).Value;
			}
		}

		/// <summary>The fully qualified name of an Activity subclass that the system can launch to let users manage the memory occupied by the application on the device.</summary>
		/// <remarks>The activity should also be declared with an <see cref="ApkActivity"/> element.</remarks>
		public String ManageSpaceActivity
		{
			get
			{
				List<String> result = base.Node.GetAttribute("manageSpaceActivity");
				return result == null
					? null
					: result[0];
			}
		}

		/// <summary>The fully qualified name of an Application subclass implemented for the application. When the application process is started, this class is instantiated before any of the application's components.</summary>
		/// <remarks>The subclass is optional; most applications won't need one. In the absence of a subclass, Android uses an instance of the base Application class.</remarks>
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

		/// <summary>Specifies the name of the XML file that contains your application's Network Security Configuration</summary>
		/// <remarks>The value must be a reference to the XML resource file containing the configuration.</remarks>
		public String NetworkSecurityConfig
		{
			get
			{
				List<String> result = base.Node.GetAttribute("networkSecurityConfig");
				return result == null
					? null
					: base.GetResource(Convert.ToInt32(result[0])).Value;
			}
		}

		/// <summary>The name of a permission that clients must have in order to interact with the application</summary>
		/// <remarks>
		/// This attribute is a convenient way to set a permission that applies to all of the application's components.
		/// It can be overwritten by setting the permission attributes of individual components.
		/// </remarks>
		public String Permission
		{
			get
			{
				List<String> result = base.Node.GetAttribute("permission");
				return result == null
					? null
					: result[0];
			}
		}

		/// <summary>Whether or not the application should remain running at all times — "true" if it should, and "false" if not.</summary>
		/// <remarks>Applications should not normally set this flag; persistence mode is intended only for certain system applications.</remarks>
		[DefaultValue(false)]
		public Boolean Persistent
		{
			get { return base.GetBooleanAttribute("persistent").GetValueOrDefault(false); }
		}

		/// <summary>The name of a process where all components of the application should run. Each component can override this default by setting its own process attribute.</summary>
		/// <remarks>
		/// By default, Android creates a process for an application when the first of its components needs to run.
		/// All components then run in that process.
		/// The name of the default process matches the package name set by the <see cref="AndroidManifest"/> element.
		/// </remarks>
		public String Process
		{
			get
			{
				List<String> result = base.Node.GetAttribute("process");
				return result == null
					? null
					: result[0];
			}
		}

		/// <summary>Indicates that the application is prepared to attempt a restore of any backed-up data set, even if the backup was stored by a newer version of the application than is currently installed on the device.</summary>
		/// <remarks>Setting this attribute to true will permit the Backup Manager to attempt restore even when a version mismatch suggests that the data are incompatible. Use with caution!</remarks>
		[DefaultValue(false)]
		public Boolean RestoreAnyVersion
		{
			get { return base.GetBooleanAttribute("restoreAnyVersion").GetValueOrDefault(false); }
		}

		/// <summary>Whether or not the application wants to opt out of scoped storage</summary>
		public Boolean? RequestLegacyExternalStorage
		{
			get { return base.GetBooleanAttribute("requestLegacyExternalStorage"); }
		}

		/// <summary>
		/// Specifies the account type required by the application in order to function.
		/// If your app requires an Account, the value for this attribute must correspond to the account authenticator type used by your app (as defined by AuthenticatorDescription), such as "com.google".
		/// </summary>
		/// <remarks>Because restricted profiles currently cannot add accounts, specifying this attribute makes your app unavailable from a restricted profile unless you also declare android:restrictedAccountType with the same value.</remarks>
		public String RequiredAccountType
		{
			get
			{
				List<String> result = base.Node.GetAttribute("requiredAccountType");
				return result == null
					? null
					: result[0];
			}
		}

		/// <summary>Specifies whether the app supports multi-window display</summary>
		/// <remarks>
		/// If you set this attribute to true, the user can launch the activity in split-screen and freeform modes.
		/// If you set the attribute to false, the activity does not support multi-window mode.
		/// If this value is false, and the user attempts to launch the activity in multi-window mode, the activity takes over the full screen.
		/// </remarks>
		[DefaultValue(true)]
		public Boolean ResizeableActivity
		{
			get { return base.GetBooleanAttribute("resizeableActivity").GetValueOrDefault(true); }
		}

		/// <summary>Specifies the account type required by this application and indicates that restricted profiles are allowed to access such accounts that belong to the owner user</summary>
		/// <remarks>If your app requires an Account and restricted profiles are allowed to access the primary user's accounts, the value for this attribute must correspond to the account authenticator type used by your app (as defined by AuthenticatorDescription), such as "com.google".</remarks>
		public String RestrictedAccountType
		{
			get
			{
				List<String> result = base.Node.GetAttribute("restrictedAccountType");
				return result == null
					? null
					: result[0];
			}
		}

		/// <summary>Declares whether your application is willing to support right-to-left (RTL) layouts</summary>
		/// <remarks>
		/// If set to true and targetSdkVersion is set to 17 or higher, various RTL APIs will be activated and used by the system so your app can display RTL layouts.
		/// If set to false or if targetSdkVersion is set to 16 or lower, the RTL APIs will be ignored or will have no effect and your app will behave the same regardless of the layout direction associated to the user's Locale choice (your layouts will always be left-to-right).
		/// </remarks>
		[DefaultValue(false)]
		public Boolean SupportsRtl
		{
			get { return base.GetBooleanAttribute("supportsRtl").GetValueOrDefault(false); }
		}

		/// <summary>
		/// An affinity name that applies to all activities within the application, except for those that set a different affinity with their own taskAffinity attributes.
		/// See that attribute for more information.
		/// </summary>
		/// <remarks>
		/// By default, all activities within an application share the same affinity.
		/// The name of that affinity is the same as the package name set by the <see cref="AndroidManifest"/> element.
		/// </remarks>
		public String TaskAffinity
		{
			get
			{
				List<String> result = base.Node.GetAttribute("taskAffinity");
				return result == null
					? null
					: result[0];
			}
		}

		/// <summary>
		/// Indicates whether this application is only for testing purposes.
		/// For example, it may expose functionality or data outside of itself that would cause a security hole, but is useful for testing.
		/// This kind of APK can be installed only through adb—you cannot publish it to Google Play.
		/// </summary>
		/// <remarks>Android Studio automatically adds this attribute when you click Run.</remarks>
		public Boolean? TestOnly
		{
			get { return base.GetBooleanAttribute("testOnly"); }
		}

		/// <summary>A reference to a style resource defining a default theme for all activities in the application</summary>
		/// <remarks>Individual activities can override the default by setting their own theme attributes</remarks>
		public String Theme
		{
			get
			{
				List<String> result = base.Node.GetAttribute("theme");
				if(result == null)
					return null;

				Arsc.ResourceRow resourceRow = base.GetResource(Convert.ToInt32(result[0]));
				if(resourceRow != null)
					return resourceRow.Value;
				else
					return result[0];
			}
		}

		/// <summary>Extra options for an activity's UI</summary>
		public uiOptionsType UIOptions
		{
			get
			{
				List<String> result = base.Node.GetAttribute("uiOptions");
				return result == null
					? uiOptionsType.none
					: (uiOptionsType)Enum.Parse(typeof(uiOptionsType), result[0]);
			}
		}

		/// <summary>
		/// Indicates whether the app intends to use cleartext network traffic, such as cleartext HTTP.
		/// The default value for apps that target API level 27 or lower is "true".
		/// Apps that target API level 28 or higher default to "false".
		/// </summary>
		/// <remarks>
		/// When the attribute is set to "false", platform components (for example, HTTP and FTP stacks, DownloadManager, and MediaPlayer) will refuse the app's requests to use cleartext traffic.
		/// Third-party libraries are strongly encouraged to honor this setting as well.
		/// The key reason for avoiding cleartext traffic is the lack of confidentiality, authenticity, and protections against tampering; a network attacker can eavesdrop on transmitted data and also modify it without being detected.
		/// </remarks>
		public Boolean? UsesCleartextTraffic
		{
			get { return base.GetBooleanAttribute("usesCleartextTraffic"); }
		}

		/// <summary>Indicates whether the app would like the virtual machine (VM) to operate in safe mode</summary>
		/// <remarks>
		/// This attribute was added in API level 8 where a value of "true" disabled the Dalvik just-in-time (JIT) compiler.
		/// This attribute was adapted in API level 22 where a value of "true" disabled the ART ahead-of-time (AOT) compiler.
		/// </remarks>
		[DefaultValue(false)]
		public Boolean VmSafeMode
		{
			get { return base.GetBooleanAttribute("vmSafeMode").GetValueOrDefault(false); }
		}

		/// <summary>
		/// Specifies a shared library that the application must be linked against.
		/// This element tells the system to include the library's code in the class loader for the package.
		/// </summary>
		/// <remarks>
		/// All of the android packages (such as android.app, android.content, android.view, and android.widget) are in the default library that all applications are automatically linked against. However, some packages (such as maps) are in separate libraries that are not automatically linked.
		/// Consult the documentation for the packages you're using to determine which library contains the package code.
		/// </remarks>
		public IEnumerable<ApkUsesLibrary> UsesLibrary
		{
			get
			{
				foreach(XmlNode node in base.Node["uses-library"])
					yield return new ApkUsesLibrary(this, node);
			}
		}

		/// <summary>Declares an activity (an Activity subclass) that implements part of the application's visual user interface</summary>
		/// <remarks>
		/// All activities must be represented by <see cref="ApkActivity"/> elements in the manifest file.
		/// Any that are not declared there will not be seen by the system and will never be run.
		/// </remarks>
		public IEnumerable<ApkActivity> Activity
		{
			get
			{
				foreach(XmlNode node in base.Node["activity"])
					yield return new ApkActivity(this, node);
			}
		}

		/// <summary>
		/// The alias presents the target activity as an independent entity.
		/// It can have its own set of intent filters, and they, rather than the intent filters on the target activity itself, determine which intents can activate the target through the alias and how the system treats the alias.
		/// For example, the intent filters on the alias may specify the "android.intent.action.MAIN" and "android.intent.category.LAUNCHER" flags, causing it to be represented in the application launcher, even though none of the filters on the target activity itself set these flags.
		/// </summary>
		/// <remarks>
		/// With the exception of targetActivity, <see cref="ApkActivityAlias"/> attributes are a subset of <see cref="ApkActivity"/> attributes.
		/// For attributes in the subset, none of the values set for the target carry over to the alias.
		/// However, for attributes not in the subset, the values set for the target activity also apply to the alias.
		/// </remarks>
		public IEnumerable<ApkActivityAlias> ActivityAlias
		{
			get
			{
				foreach(XmlNode node in base.Node["activity-alias"])
					yield return new ApkActivityAlias(this, node);
			}
		}

		/// <summary>
		/// Declares a broadcast receiver (a BroadcastReceiver subclass) as one of the application's components.
		/// Broadcast receivers enable applications to receive intents that are broadcast by the system or by other applications, even when other components of the application are not running.
		/// </summary>
		/// <remarks>
		/// There are two ways to make a broadcast receiver known to the system: One is declare it in the manifest file with this element.
		/// The other is to create the receiver dynamically in code and register it with the Context.registerReceiver() method.
		/// For more information about how to dynamically create receivers, see the BroadcastReceiver class description.
		/// </remarks>
		public IEnumerable<ApkReceiver> Reciever
		{
			get
			{
				foreach(XmlNode node in base.Node["receiver"])
					yield return new ApkReceiver(this, node);
			}
		}

		/// <summary>Declares a service (a Service subclass) as one of the application's components</summary>
		/// <remarks>
		/// Unlike activities, services lack a visual user interface.
		/// They're used to implement long-running background operations or a rich communications API that can be called by other applications.
		/// 
		/// All services must be represented by <see cref="ApkService"/> elements in the manifest file. Any that are not declared there will not be seen by the system and will never be run.
		/// </remarks>
		public IEnumerable<ApkService> Service
		{
			get
			{
				foreach(XmlNode node in base.Node["service"])
					yield return new ApkService(this, node);
			}
		}

		/// <summary>A name-value pair for an item of additional, arbitrary data that can be supplied to the parent component.</summary>
		/// <remarks>
		/// A component element can contain any number of <see cref="ApkMetaData"/> subelements.
		/// The values from all of them are collected in a single Bundle object and made available to the component as the PackageItemInfo.metaData field.
		/// </remarks>
		public IEnumerable<ApkMetaData> MetaData
		{
			get
			{
				foreach(XmlNode node in base.Node["meta-data"])
					yield return new ApkMetaData(this, node);
			}
		}

		/// <summary>Declares a content provider component</summary>
		/// <remarks>
		/// A content provider is a subclass of ContentProvider that supplies structured access to data managed by the application.
		/// All content providers in your application must be defined in a <see cref="ApkProvider"/> element in the manifest file; otherwise, the system is unaware of them and doesn't run them.
		/// </remarks>
		public IEnumerable<ApkProvider> Provider
		{
			get
			{
				foreach(XmlNode node in base.Node["provider"])
					yield return new ApkProvider(this, node);
			}
		}

		internal ApkApplication(AndroidManifest manifest)
			: base(manifest, manifest.Node.ChildNodes["application"][0])
		{
		}
	}
}