using System;
using System.Collections.Generic;
using System.ComponentModel;
using AlphaOmega.Debug.Arsc;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Declares an activity (an Activity subclass) that implements part of the application's visual user interface</summary>
	/// <remarks>
	/// All activities must be represented by <see cref="ApkActivity"/> elements in the manifest file.
	/// Any that are not declared there will not be seen by the system and will never be run.
	/// </remarks>
	public class ApkActivity : ApkNodeT<ApkApplication>, IApkIntentedNode
	{
		/// <summary>Requests the activity to be displayed in wide color gamut mode on compatible devices.</summary>
		public enum ColorModeType
		{
			/// <summary>If the device does not support wide color gamut rendering, this attribute has no effect</summary>
			hdr,
			/// <summary>In wide color gamut mode, a window can render outside of the sRGB gamut to display more vibrant colors</summary>
			wideColorGamut,
		}

		/// <summary>Lists configuration changes that the activity will handle itself</summary>
		public enum ConfigChangesType
		{
			/// <summary>The display density has changed — the user might have specified a different display scale, or a different display might now be active.</summary>
			/// <remarks>Added in API level 24.</remarks>
			density,
			/// <summary>The font scaling factor has changed — the user has selected a new global font size.</summary>
			fontScale,
			/// <summary>The keyboard type has changed — for example, the user has plugged in an external keyboard</summary>
			keyboard,
			/// <summary>The keyboard accessibility has changed — for example, the user has revealed the hardware keyboard</summary>
			keyboardHidden,
			/// <summary>The layout direction has changed — for example, changing from left-to-right (LTR) to right-to-left (RTL).</summary>
			/// <remarks>Added in API level 17.</remarks>
			layoutDirection,
			/// <summary>The locale has changed — the user has selected a new language that text should be displayed in.</summary>
			locale,
			/// <summary>The IMSI mobile country code (MCC) has changed — a SIM has been detected and updated the MCC.</summary>
			mcc,
			/// <summary>The IMSI mobile network code (MNC) has changed — a SIM has been detected and updated the MNC.</summary>
			mnc,
			/// <summary>The navigation type (trackball/dpad) has changed. (This should never normally happen.)</summary>
			navigation,
			/// <summary>The screen orientation has changed — the user has rotated the device.</summary>
			/// <remarks>If your application targets Android 3.2 (API level 13) or higher, then you should also declare the "screenSize" and "screenLayout" configurations, because they might also change when a device switches between portrait and landscape orientations</remarks>
			orientation,
			/// <summary>The screen layout has changed — a different display might now be active.</summary>
			screenLayout,
			/// <summary>
			/// The current available screen size has changed.
			/// This represents a change in the currently available size, relative to the current aspect ratio, so will change when the user switches between landscape and portrait.
			/// </summary>
			/// <remarks>Added in API level 13</remarks>
			screenSize,
			/// <summary>
			/// The physical screen size has changed.
			/// This represents a change in size regardless of orientation, so will only change when the actual physical screen size has changed such as switching to an external display.
			/// A change to this configuration corresponds to a change in the smallestWidth configuration.
			/// </summary>
			/// <remarks>Added in API level 13.</remarks>
			smallestScreenSize,
			/// <summary>The touchscreen has changed. (This should never normally happen.)</summary>
			touchscreen,
			/// <summary>
			/// The user interface mode has changed — the user has placed the device into a desk or car dock, or the night mode has changed.
			/// For more information about the different UI modes, see UiModeManager.
			/// </summary>
			/// <remarks>Added in API level 8.</remarks>
			uiMode,
		}

		/// <summary>Specifies how a new instance of an activity should be added to a task each time it is launched.</summary>
		public enum DocumentLaunchModeType
		{
			/// <summary>
			/// The system searches for a task whose base intent's ComponentName and data URI match those of the launching intent.
			/// If the system finds such a task, the system clears the task, and restarts with the root activity receiving a call to onNewIntent(android.content.Intent).
			/// If the system does not find such a task, the system creates a new task.
			/// </summary>
			intoExisting,
			/// <summary>
			/// The activity creates a new task for the document, even if the document is already opened.
			/// This is the same as setting both the FLAG_ACTIVITY_NEW_DOCUMENT and FLAG_ACTIVITY_MULTIPLE_TASK flags.
			/// </summary>
			always,
			/// <summary>
			/// The activity does not create a new task for the activity.
			/// This is the default value, which creates a new task only when FLAG_ACTIVITY_NEW_TASK is set.
			/// The overview screen treats the activity as it would by default: it displays a single task for the app, which resumes from whatever activity the user last invoked.
			/// </summary>
			none,
			/// <summary>
			/// This activity is not launched into a new document even if the Intent contains FLAG_ACTIVITY_NEW_DOCUMENT.
			/// Setting this overrides the behavior of the FLAG_ACTIVITY_NEW_DOCUMENT and FLAG_ACTIVITY_MULTIPLE_TASK flags, if either of these are set in the activity, and the overview screen displays a single task for the app, which resumes from whatever activity the user last invoked.
			/// </summary>
			never,
		}

		/// <summary>An instruction on how the activity should be launched</summary>
		public enum LaunchModeType
		{
			/// <summary>Default. The system always creates a new instance of the activity in the target task and routes the intent to it.</summary>
			standard,
			/// <summary>If an instance of the activity already exists at the top of the target task, the system routes the intent to that instance through a call to its onNewIntent() method, rather than creating a new instance of the activity.</summary>
			singleTop,
			/// <summary>
			/// The system creates the activity at the root of a new task and routes the intent to it.
			/// However, if an instance of the activity already exists, the system routes the intent to existing instance through a call to its onNewIntent() method, rather than creating a new one.
			/// </summary>
			singleTask,
			/// <summary>
			/// Same as "singleTask", except that the system doesn't launch any other activities into the task holding the instance.
			/// The activity is always the single and only member of its task.
			/// </summary>
			singleInstance,
		}

		/// <summary>Determines how the system presents this activity when the device is running in lock task mode</summary>
		public enum LockTaskModeType
		{
			/// <summary>This is the default value. Tasks don't launch into lock task mode but can be placed there by calling startLockTask()</summary>
			normal,
			/// <summary>Tasks don't launch into lockTask mode, and the device user can't pin these tasks from the overview screen</summary>
			/// <remarks>This mode is only available to system and privileged applications. Non-privileged apps with this value are treated as normal</remarks>
			never,
			/// <summary>
			/// If the DPC authorizes this package using DevicePolicyManager.setLockTaskPackages(), then this mode is identical to always, except that the activity needs to call stopLockTask() before being able to finish if it is the last locked task.
			/// If the DPC does not authorize this package then this mode is identical to normal.
			/// </summary>
			if_whitelisted,
			/// <summary>
			/// Tasks rooted at this activity always launch into lock task mode.
			/// If the system is already in lock task mode when this task is launched then the new task are launched on top of the current task.
			/// Tasks launched in this mode can exit lock task mode by calling finish()
			/// </summary>
			/// <remarks>This mode is only available to system and privileged applications. Non-privileged apps with this value are treated as normal</remarks>
			always,
		}

		/// <summary>Defines how an instance of an activity is preserved within a containing task across device restarts</summary>
		public enum PersistableModeType
		{
			/// <summary>
			/// When the system restarts, the activity task is preserved, but only the root activity's launching intent is used.
			/// 
			/// When your app's launching intent loads your app's root activity, the activity doesn't receive a PersistableBundle object.
			/// Therefore, don't use onSaveInstanceState() to preserve the state of your app's root activity across a device restart.
			/// </summary>
			/// <remarks>This attribute value affects your app's behavior only if it's set on your app's root activity</remarks>
			persistRootOnly,
			/// <summary>
			/// This activity's state is preserved, along with the state of each activity higher up the back stack that has its own persistableMode attribute set to persistAcrossReboots.
			/// If an activity doesn't have a persistableMode attribute that is set to persistAcrossReboots, or if it's launched using the Intent.FLAG_ACTIVITY_NEW_DOCUMENT flag, then that activity, along with all activities higher up the back stack, aren't preserved.
			/// 
			/// When an intent loads an activity whose persistableMode attribute is set to persistAcrossReboots in your app, the activity receives a PersistableBundle object in its onCreate() method.
			/// Therefore, you can use onSaveInstanceState() to preserve the state of an activity across a device restart as long as its persistableMode attribute is set to persistAcrossReboots.
			/// </summary>
			/// <remarks>This attribute value affects your app's behavior even if it's set on an activity other than your app's root activity</remarks>
			persistAcrossReboots,
			/// <summary>The activity's state isn't preserved</summary>
			/// <remarks>This attribute value affects your app's behavior only if it's set on your app's root activity.</remarks>
			persistNever,
		}

		/// <summary>The orientation of the activity's display on the device</summary>
		/// <remarks>
		/// When you declare one of the landscape or portrait values, it is considered a hard requirement for the orientation in which the activity runs.
		/// As such, the value you declare enables filtering by services such as Google Play so your application is available only to devices that support the orientation required by your activities.
		/// For example, if you declare either "landscape", "reverseLandscape", or "sensorLandscape", then your application will be available only to devices that support landscape orientation.
		/// However, you should also explicitly declare that your application requires either portrait or landscape orientation with the <see cref="ApkUsesFeature"/> element.
		/// For example, &lt;uses-feature android:name="android.hardware.screen.portrait"/&gt;.
		/// This is purely a filtering behavior provided by Google Play (and other services that support it) and the platform itself does not control whether your app can be installed when a device supports only certain orientations
		/// </remarks>
		public enum ScreenOrientationType
		{
			/// <summary>
			/// The default value. The system chooses the orientation. The policy it uses, and therefore the choices made in specific contexts, may differ from device to device
			/// </summary>
			unspecified,
			/// <summary>The same orientation as the activity that's immediately beneath it in the activity stack</summary>
			behind,
			/// <summary>Landscape orientation (the display is wider than it is tall)</summary>
			landscape,
			/// <summary>Portrait orientation (the display is taller than it is wide)</summary>
			portrait,
			/// <summary>Landscape orientation in the opposite direction from normal landscape</summary>
			/// <remarks>Added in API level 9</remarks>
			reverseLandscape,
			/// <summary>Portrait orientation in the opposite direction from normal portrait</summary>
			/// <remarks>Added in API level 9</remarks>
			reversePortrait,
			/// <summary>Landscape orientation, but can be either normal or reverse landscape based on the device sensor. The sensor is used even if the user has locked sensor-based rotation</summary>
			/// <remarks>Added in API level 9</remarks>
			sensorLandscape,
			/// <summary>Portrait orientation, but can be either normal or reverse portrait based on the device sensor. The sensor is used even if the user has locked sensor-based rotation</summary>
			/// <remarks>Added in API level 9</remarks>
			sensorPortrait,
			/// <summary>Landscape orientation, but can be either normal or reverse landscape based on the device sensor and the user's preference</summary>
			/// <remarks>Added in API level 18</remarks>
			userLandscape,
			/// <summary>Portrait orientation, but can be either normal or reverse portrait based on the device sensor and the user's preference</summary>
			/// <remarks>Added in API level 18</remarks>
			userPortrait,
			/// <summary>
			/// The orientation is determined by the device orientation sensor.
			/// The orientation of the display depends on how the user is holding the device; it changes when the user rotates the device.
			/// Some devices, though, will not rotate to all four possible orientations, by default.
			/// To allow all four orientations, use "fullSensor" The sensor is used even if the user locked sensor-based rotation.
			/// </summary>
			sensor,
			/// <summary>
			/// The orientation is determined by the device orientation sensor for any of the 4 orientations.
			/// This is similar to "sensor" except this allows any of the 4 possible screen orientations, regardless of what the device will normally do (for example, some devices won't normally use reverse portrait or reverse landscape, but this enables those)
			/// </summary>
			/// <remarks>Added in API level 9</remarks>
			fullSensor,
			/// <summary>
			/// The orientation is determined without reference to a physical orientation sensor.
			/// The sensor is ignored, so the display will not rotate based on how the user moves the device.
			/// </summary>
			nosensor,
			/// <summary>The user's current preferred orientation</summary>
			user,
			/// <summary>If the user has locked sensor-based rotation, this behaves the same as user, otherwise it behaves the same as fullSensor and allows any of the 4 possible screen orientations</summary>
			/// <remarks>Added in API level 18</remarks>
			fullUser,
			/// <summary>Locks the orientation to its current rotation, whatever that is</summary>
			/// <remarks>Added in API level 18</remarks>
			locked,
		}

		/// <summary>Extra options for an activity's UI</summary>
		public enum uiOptonsType
		{
			/// <summary>No extra UI options</summary>
			none,
			/// <summary>
			/// Add a bar at the bottom of the screen to display action items in the app bar (also known as the action bar), when constrained for horizontal space (such as when in portrait mode on a handset).
			/// Instead of a small number of action items appearing in the app bar at the top of the screen, the app bar is split into the top navigation section and the bottom bar for action items.
			/// This ensures a reasonable amount of space is made available not only for the action items, but also for navigation and title elements at the top.
			/// Menu items are not split across the two bars; they always appear together
			/// </summary>
			splitActionBarWhenNarrow,
		}

		/// <summary>How the main window of the activity interacts with the window containing the on-screen soft keyboard</summary>
		public enum WindowSoftInputModeType
		{
			/// <summary>
			/// The state of the soft keyboard (whether it is hidden or visible) is not specified.
			/// The system will choose an appropriate state or rely on the setting in the theme.
			/// This is the default setting for the behavior of the soft keyboard.
			/// </summary>
			stateUnspecified,
			/// <summary>The soft keyboard is kept in whatever state it was last in, whether visible or hidden, when the activity comes to the fore</summary>
			stateUnchanged,
			/// <summary>The soft keyboard is hidden when the user chooses the activity — that is, when the user affirmatively navigates forward to the activity, rather than backs into it because of leaving another activity</summary>
			stateHidden,
			/// <summary>The soft keyboard is always hidden when the activity's main window has input focus</summary>
			stateAlwaysHidden,
			/// <summary>The soft keyboard is visible when that's normally appropriate (when the user is navigating forward to the activity's main window).</summary>
			stateVisible,
			/// <summary>The soft keyboard is made visible when the user chooses the activity — that is, when the user affirmatively navigates forward to the activity, rather than backs into it because of leaving another activity</summary>
			stateAlwaysVisible,
			/// <summary>
			/// It is unspecified whether the activity's main window resizes to make room for the soft keyboard, or whether the contents of the window pan to make the current focus visible on-screen.
			/// The system will automatically select one of these modes depending on whether the content of the window has any layout views that can scroll their contents.
			/// If there is such a view, the window will be resized, on the assumption that scrolling can make all of the window's contents visible within a smaller area.
			/// This is the default setting for the behavior of the main window.
			/// </summary>
			adjustUnspecified,
			/// <summary>The activity's main window is always resized to make room for the soft keyboard on screen.</summary>
			adjustResize,
			/// <summary>
			/// The activity's main window is not resized to make room for the soft keyboard.
			/// Rather, the contents of the window are automatically panned so that the current focus is never obscured by the keyboard and users can always see what they are typing.
			/// This is generally less desirable than resizing, because the user may need to close the soft keyboard to get at and interact with obscured parts of the window.
			/// </summary>
			adjustPan,
		}

		/// <summary>Indicate that the activity can be launched as the embedded child of another activity.</summary>
		/// <remarks>
		/// Particularly in the case where the child lives in a container such as a Display owned by another activity.
		/// For example, activities that are used for Wear custom notifications must declare this so Wear can display the activity in it's context stream, which resides in another process.
		/// </remarks>
		[DefaultValue(false)]
		public Boolean AllowEmbedded
			=> base.GetBooleanAttribute("allowEmbedded").GetValueOrDefault(false);

		/// <summary>
		/// Whether or not the activity can move from the task that started it to the task it has an affinity for when that task is next brought to the front — "true" if it can move, and "false" if it must remain with the task where it started.
		/// If this attribute is not set, the value set by the corresponding allowTaskReparenting attribute of the <see cref="ApkApplication"/> element applies to the activity.
		/// </summary>
		/// <remarks>
		/// Normally when an activity is started, it's associated with the task of the activity that started it and it stays there for its entire lifetime.
		/// You can use this attribute to force it to be re-parented to the task it has an affinity for when its current task is no longer displayed.
		/// Typically, it's used to cause the activities of an application to move to the main task associated with that application.
		/// 
		/// For example, if an e-mail message contains a link to a web page, clicking the link brings up an activity that can display the page.
		/// That activity is defined by the browser application, but is launched as part of the e-mail task.
		/// If it's reparented to the browser task, it will be shown when the browser next comes to the front, and will be absent when the e-mail task again comes forward.
		/// 
		/// The affinity of an activity is defined by the taskAffinity attribute.
		/// The affinity of a task is determined by reading the affinity of its root activity.
		/// Therefore, by definition, a root activity is always in a task with the same affinity.
		/// Since activities with "singleTask" or "singleInstance" launch modes can only be at the root of a task, re-parenting is limited to the "standard" and "singleTop" modes.
		/// (See also the launchMode attribute.)
		/// </remarks>
		[DefaultValue(false)]
		public Boolean AllowTaskReparenting
			=> base.GetBooleanAttribute("allowTaskReparenting").GetValueOrDefault(false);

		/// <summary>
		/// Whether or not the state of the task that the activity is in will always be maintained by the system — "true" if it will be, and "false" if the system is allowed to reset the task to its initial state in certain situations.
		/// This attribute is meaningful only for the root activity of a task; it's ignored for all other activities.
		/// </summary>
		/// <remarks>
		/// Normally, the system clears a task (removes all activities from the stack above the root activity) in certain situations when the user re-selects that task from the home screen. Typically, this is done if the user hasn't visited the task for a certain amount of time, such as 30 minutes.
		/// However, when this attribute is "true", users will always return to the task in its last state, regardless of how they get there. This is useful, for example, in an application like the web browser where there is a lot of state (such as multiple open tabs) that users would not like to lose. 
		/// </remarks>
		[DefaultValue(false)]
		public Boolean AlwaysRetainTaskState
			=> base.GetBooleanAttribute("alwaysRetainTaskState").GetValueOrDefault(false);

		/// <summary>Whether or not tasks launched by activities with this attribute remains in the overview screen until the last activity in the task is completed.</summary>
		/// <remarks>
		/// If true, the task is automatically removed from the overview screen.
		/// This overrides the caller's use of FLAG_ACTIVITY_RETAIN_IN_RECENTS.
		/// It must be a boolean value, either "true" or "false".
		/// </remarks>
		public Boolean? AutoRemoveFromRecents
			=> base.GetBooleanAttribute("autoRemoveFromRecents");

		/// <summary>The system uses the banner to represent an app in the Android TV home screen.</summary>
		/// <remarks>
		/// Since the banner is displayed only in the home screen, it should only be specified by applications with an activity that handles the CATEGORY_LEANBACK_LAUNCHER intent.
		/// </remarks>
		public String Banner
		{
			get
			{
				List<String> result = base.Node.GetAttribute("banner");
				return result == null
					? base.ParentNode.Banner
					: base.GetResource(Convert.ToInt32(result[0])).Value;
			}
		}

		/// <summary>
		/// Whether or not all activities will be removed from the task, except for the root activity, whenever it is re-launched from the home screen — "true" if the task is always stripped down to its root activity, and "false" if not.
		/// This attribute is meaningful only for activities that start a new task (the root activity); it's ignored for all other activities in the task.
		/// </summary>
		/// <remarks>
		/// When the value is "true", every time users start the task again, they are brought to its root activity regardless of what they were last doing in the task and regardless of whether they used the Back or Home button to leave it.
		/// When the value is "false", the task may be cleared of activities in some situations (see the alwaysRetainTaskState attribute), but not always.
		/// 
		/// Suppose, for example, that someone launches activity P from the home screen, and from there goes to activity Q.
		/// The user next presses Home, and then returns to activity P.
		/// Normally, the user would see activity Q, since that is what they were last doing in P's task.
		/// However, if P set this flag to "true", all of the activities on top of it (Q in this case) were removed when the user pressed Home and the task went to the background.
		/// So the user sees only P when returning to the task.
		/// 
		/// If this attribute and allowTaskReparenting are both "true", any activities that can be re-parented are moved to the task they share an affinity with; the remaining activities are then dropped, as described above.
		/// </remarks>
		[DefaultValue(false)]
		public Boolean ClearTaskOnLaunch
			=> base.GetBooleanAttribute("clearTaskOnLaunch").GetValueOrDefault(false);

		/// <summary>Requests the activity to be displayed in wide color gamut mode on compatible devices</summary>
		/// <remarks>
		/// In wide color gamut mode, a window can render outside of the SRGB gamut to display more vibrant colors.
		/// If the device doesn't support wide color gamut rendering, this attribute has no effect.
		/// For more information about rendering in wide color mode, see Enhancing Graphics with Wide Color Content.
		/// </remarks>
		public ColorModeType? ColorMode
		{
			get
			{
				List<String> result = base.Node.GetAttribute("colorMode");
				return result == null
					? (ColorModeType?)null
					: (ColorModeType)Enum.Parse(typeof(ColorModeType), result[0]);
			}
		}

		/// <summary>
		/// Lists configuration changes that the activity will handle itself.
		/// When a configuration change occurs at runtime, the activity is shut down and restarted by default, but declaring a configuration with this attribute will prevent the activity from being restarted.
		/// Instead, the activity remains running and its onConfigurationChanged() method is called.
		/// </summary>
		/// <remarks>
		/// Using this attribute should be avoided and used only as a last resort.
		/// Please read Handling Runtime Changes for more information about how to properly handle a restart due to a configuration change.
		/// </remarks>
		public ConfigChangesType[] configChanges
		{
			get
			{
				List<String> result = base.Node.GetAttribute("configChanges");
				return result == null
					? new ConfigChangesType[] { }
					: Array.ConvertAll(result[0].Split('|'), delegate(String item) { return (ConfigChangesType)Enum.Parse(typeof(ConfigChangesType), item); });
			}
		}

		/// <summary>Whether or not the activity is direct-boot aware; that is, whether or not it can run before the user unlocks the device.</summary>
		/// <remarks>During Direct Boot, an activity in your application can only access the data that is stored in device protected storage</remarks>
		[DefaultValue(false)]
		public Boolean DirectBootAware
			=> base.GetBooleanAttribute("directBootAware").GetValueOrDefault(false);

		/// <summary>
		/// Specifies how a new instance of an activity should be added to a task each time it is launched.
		/// This attribute permits the user to have multiple documents from the same application appear in the overview screen.
		/// </summary>
		/// <remarks>
		/// For values other than "none" and "never" the activity must be defined with launchMode="standard".
		/// If this attribute is not specified, documentLaunchMode="none" is used.
		/// </remarks>
		public DocumentLaunchModeType DocumentLaunchMode
		{
			get
			{
				List<String> result = base.Node.GetAttribute("documentLaunchMode");
				return result == null
					? DocumentLaunchModeType.none
					: (DocumentLaunchModeType)Enum.Parse(typeof(DocumentLaunchModeType), result[0]);
			}
		}

		/// <summary>Whether or not the activity can be instantiated by the system — "true" if it can be, and "false" if not.</summary>
		/// <remarks>
		/// The <see cref="ApkApplication"/> element has its own enabled attribute that applies to all application components, including activities.
		/// The <see cref="ApkApplication"/> and <see cref="ApkActivity"/> attributes must both be "true" (as they both are by default) for the system to be able to instantiate the activity. If either is "false", it cannot be instantiated.
		/// </remarks>
		[DefaultValue(true)]
		public Boolean Enabled
			=> base.GetBooleanAttribute("enabled").GetValueOrDefault(true);

		/// <summary>Whether or not the task initiated by this activity should be excluded from the list of recently used applications, the overview screen.</summary>
		/// <remarks>
		/// That is, when this activity is the root activity of a new task, this attribute determines whether the task should not appear in the list of recent apps.
		/// Set "true" if the task should be excluded from the list; set "false" if it should be included.
		/// </remarks>
		[DefaultValue(false)]
		public Boolean ExcludeFromRecents
			=> base.GetBooleanAttribute("excludeFromRecents").GetValueOrDefault(false);

		/// <summary>
		/// This element sets whether the activity can be launched by components of other applications — "true" if it can be, and "false" if not. If "false", the activity can be launched only by components of the same application or applications with the same user ID.
		/// </summary>
		/// <remarks>
		/// If you are using intent filters, you should not set this element "false".
		/// If you do so, and an app tries to call the activity, system throws an ActivityNotFoundException.
		/// Instead, you should prevent other apps from calling the activity by not setting intent filters for it.
		/// 
		/// If you do not have intent filters, the default value for this element is "false".
		/// If you set the element "true", the activity is accessible to any app that knows its exact class name, but does not resolve when the system tries to match an implicit intent.
		/// 
		/// This attribute is not the only way to limit an activity's exposure to other applications. You can also use a permission to limit the external entities that can invoke the activity (see the permission attribute).
		/// </remarks>
		public Boolean? Exported
			=> base.GetBooleanAttribute("exported");

		/// <summary>Whether or not an existing instance of the activity should be shut down (finished) whenever the user again launches its task (chooses the task on the home screen) — "true" if it should be shut down, and "false" if not.</summary>
		/// <remarks>
		/// If this attribute and allowTaskReparenting are both "true", this attribute trumps the other.
		/// The affinity of the activity is ignored.
		/// The activity is not re-parented, but destroyed.
		/// </remarks>
		[DefaultValue(false)]
		public Boolean FinishOnTaskLaunch
			=> base.GetBooleanAttribute("finishOnTaskLaunch").GetValueOrDefault(false);

		/// <summary>Whether or not hardware-accelerated rendering should be enabled for this Activity — "true" if it should be enabled, and "false" if not.</summary>
		/// <remarks>
		/// Starting from Android 3.0, a hardware-accelerated OpenGL renderer is available to applications, to improve performance for many common 2D graphics operations.
		/// When the hardware-accelerated renderer is enabled, most operations in Canvas, Paint, Xfermode, ColorFilter, Shader, and Camera are accelerated.
		/// This results in smoother animations, smoother scrolling, and improved responsiveness overall, even for applications that do not explicitly make use the framework's OpenGL libraries.
		/// Because of the increased resources required to enable hardware acceleration, your app will consume more RAM.
		/// 
		/// Note that not all of the OpenGL 2D operations are accelerated. If you enable the hardware-accelerated renderer, test your application to ensure that it can make use of the renderer without errors.
		/// </remarks>
		[DefaultValue(false)]
		public Boolean HardwareAccelerated
			=> base.GetBooleanAttribute("hardwareAccelerated").GetValueOrDefault(false);

		/// <summary>An icon representing the activity</summary>
		/// <remarks>
		/// The icon is displayed to users when a representation of the activity is required on-screen.
		/// For example, icons for activities that initiate tasks are displayed in the launcher window.
		/// The icon is often accompanied by a label (see the android:label attribute).
		/// </remarks>
		public String Icon
		{
			get
			{
				List<String> result = base.Node.GetAttribute("icon");
				return result == null
					? base.ParentNode.Icon
					: base.GetResource(Convert.ToInt32(result[0])).Value;
			}
		}

		/// <summary>Sets the immersive mode setting for the current activity</summary>
		/// <remarks>
		/// If the android:immersive attribute is set to true in the app's manifest entry for this activity, the ActivityInfo.flags member always has its FLAG_IMMERSIVE bit set, even if the immersive mode is changed at runtime using the setImmersive() method.
		/// </remarks>
		public Boolean? Immersive
			=> base.GetBooleanAttribute("immersive");

		/// <summary>A user-readable label for the activity</summary>
		/// <remarks>
		/// The label is displayed on-screen when the activity must be represented to the user.
		/// It's often displayed along with the activity icon.
		/// </remarks>
		/// <see cref="ApkApplication.Label"/>
		public String Label
		{
			get
			{
				List<String> result = base.Node.GetAttribute("label");
				if(result == null)
					return base.ParentNode.Label;

				if(Int32.TryParse(result[0], out Int32 resourceId))
				{
					ResourceRow resource = base.GetResource(resourceId);
					if(resource != null)
						return resource.Value;
				}
				return result[0];
			}
		}

		/// <summary>
		/// An instruction on how the activity should be launched.
		/// There are four modes that work in conjunction with activity flags (FLAG_ACTIVITY_* constants) in Intent objects to determine what should happen when the activity is called upon to handle an intent.
		/// </summary>
		/// <remarks>
		/// As shown in the table below, the modes fall into two main groups, with "standard" and "singleTop" activities on one side, and "singleTask" and "singleInstance" activities on the other. An activity with the "standard" or "singleTop" launch mode can be instantiated multiple times.
		/// The instances can belong to any task and can be located anywhere in the activity stack.
		/// Typically, they're launched into the task that called startActivity() (unless the Intent object contains a FLAG_ACTIVITY_NEW_TASK instruction, in which case a different task is chosen — see the taskAffinity attribute).
		/// 
		/// In contrast, "singleTask" and "singleInstance" activities can only begin a task.
		/// They are always at the root of the activity stack.
		/// Moreover, the device can hold only one instance of the activity at a time — only one such task.
		/// 
		/// The "standard" and "singleTop" modes differ from each other in just one respect:
		/// Every time there's a new intent for a "standard" activity, a new instance of the class is created to respond to that intent.
		/// Each instance handles a single intent.
		/// Similarly, a new instance of a "singleTop" activity may also be created to handle a new intent.
		/// However, if the target task already has an existing instance of the activity at the top of its stack, that instance will receive the new intent (in an onNewIntent() call); a new instance is not created.
		/// In other circumstances — for example, if an existing instance of the "singleTop" activity is in the target task, but not at the top of the stack, or if it's at the top of a stack, but not in the target task — a new instance would be created and pushed on the stack.
		/// </remarks>
		[DefaultValue(LaunchModeType.standard)]
		public LaunchModeType LaunchMode
		{
			get
			{
				List<String> result = base.Node.GetAttribute("launchMode");
				return result == null
					? LaunchModeType.standard
					: (LaunchModeType)Enum.Parse(typeof(LaunchModeType), result[0]);
			}
		}

		/// <summary>Determines how the system presents this activity when the device is running in lock task mode</summary>
		/// <remarks>
		/// Android can run tasks in an immersive, kiosk-like fashion called lock task mode.
		/// When the system runs in lock task mode, device users typically can’t see notifications, access non-whitelisted apps, or return to the home screen (unless the Home app is whitelisted).
		/// Only apps that have been whitelisted by a device policy controller (DPC) can run when the system is in lock task mode.
		/// System and privileged apps, however, can run in lock task mode without being whitelisted.
		/// </remarks>
		[DefaultValue(LockTaskModeType.normal)]
		public LockTaskModeType LockTaskMode
		{
			get
			{
				List<String> result = base.Node.GetAttribute("lockTaskMode");
				return result == null
					? LockTaskModeType.normal
					: (LockTaskModeType)Enum.Parse(typeof(LockTaskModeType), result[0]);
			}
		}

		/// <summary>The maximum number of tasks rooted at this activity in the overview screen</summary>
		/// <remarks>
		/// When this number of entries is reached, the system removes the least-recently used instance from the overview screen.
		/// Valid values are 1 through 50 (25 on low memory devices); zero is invalid.
		/// This must be an integer value, such as 50.
		/// </remarks>
		[DefaultValue(16)]
		public Int32 MaxRecents
		{
			get
			{
				List<String> result = base.Node.GetAttribute("maxRecents");
				return result == null
					? 16
					: Int32.Parse(result[0]);
			}
		}

		/// <summary>The maximum aspect ratio the activity supports</summary>
		/// <remarks>
		/// If the app runs on a device with a wider aspect ratio, the system automatically letterboxes the app, leaving portions of the screen unused so the app can run at its specified maximum aspect ratio.
		/// Maximum aspect ratio is expressed as the decimal form of the quotient of the device's longer dimension divided by its shorter dimension.
		/// For example, if the maximum aspect ratio is 7:3, set the value of this attribute to 2.33.
		/// On non-wearable devices, the value of this attribute needs to be 1.33 or greater.
		/// On wearable devices, it must be 1.0 or greater.
		/// Otherwise, the system ignores the set value.
		/// </remarks>
		public Single? MaxAspectRatio
		{
			get
			{
				List<String> result = base.Node.GetAttribute("maxAspectRatio");
				return result == null
					? (Single?)null
					: Single.Parse(result[0]);
			}
		}
		/// <summary>Whether an instance of the activity can be launched into the process of the component that started it — "true" if it can be, and "false" if not.</summary>
		/// <remarks>
		/// Normally, a new instance of an activity is launched into the process of the application that defined it, so all instances of the activity run in the same process.
		/// However, if this flag is set to "true", instances of the activity can run in multiple processes, allowing the system to create instances wherever they are used (provided permissions allow it), something that is almost never necessary or desirable.
		/// </remarks>
		[DefaultValue(false)]
		public Boolean Multiprocess
			=> base.GetBooleanAttribute("multiprocess").GetValueOrDefault(false);

		/// <summary>The name of the class that implements the activity, a subclass of Activity</summary>
		/// <remarks>Once you publish your application, you should not change this name (unless you've set android:exported="false").</remarks>
		public String Name
		{
			get
			{
				String activityName = base.Node.Attributes["name"][0];
				return activityName.StartsWith(".")
					? base.ParentNode.ParentNode.Package + activityName
					: activityName;
			}
		}

		/// <summary>Whether or not the activity should be removed from the activity stack and finished (its finish() method called) when the user navigates away from it and it's no longer visible on screen — "true" if it should be finished, and "false" if not.</summary>
		/// <remarks>
		/// A value of "true" means that the activity will not leave a historical trace.
		/// It will not remain in the activity stack for the task, so the user will not be able to return to it. In this case, onActivityResult() is never called if you start another activity for a result from this activity.
		/// </remarks>
		[DefaultValue(false)]
		public Boolean NoHistory
			=> base.GetBooleanAttribute("noHistory").GetValueOrDefault(false);

		/// <summary>
		/// The class name of the logical parent of the activity.
		/// The name here must match the class name given to the corresponding <see cref="ApkActivity"/> element's android:name attribute
		/// </summary>
		/// <remarks>
		/// The system reads this attribute to determine which activity should be started when the user presses the Up button in the action bar.
		/// The system can also use this information to synthesize a back stack of activities with TaskStackBuilder.
		/// </remarks>
		public String ParentActivityName
		{
			get
			{
				List<String> result = base.Node.GetAttribute("parentActivityName");
				if(result == null)
					return null;
				else
					return result[0].StartsWith(".")
						? base.ParentNode.ParentNode.Package + result[0]
						: result[0];
			}
		}

		/// <summary>Defines how an instance of an activity is preserved within a containing task across device restarts</summary>
		/// <remarks>
		/// If the root activity of a task sets this attribute's value to persistRootOnly, then only the root activity is preserved.
		/// Otherwise, the activities that are higher up the task's back stack are examined; any of these activities that set this attribute's value to persistAcrossReboots are preserved.
		/// </remarks>
		public PersistableModeType PersistableMode
		{
			get
			{
				List<String> result = base.Node.GetAttribute("persistableMode");
				return result == null
					? PersistableModeType.persistRootOnly
					: (PersistableModeType)Enum.Parse(typeof(PersistableModeType), result[0]);
			}
		}

		/// <summary>
		/// The name of a permission that clients must have to launch the activity or otherwise get it to respond to an intent.
		/// If a caller of startActivity() or startActivityForResult() has not been granted the specified permission, its intent will not be delivered to the activity.
		/// </summary>
		/// <remarks>
		/// If this attribute is not set, the permission set by the <see cref="ApkApplication"/> element's permission attribute applies to the activity.
		/// If neither attribute is set, the activity is not protected by a permission.
		/// </remarks>
		public String Permission
		{
			get
			{
				List<String> result = base.Node.GetAttribute("permission");
				return result == null
					? base.ParentNode.Permission
					: result[0];
			}
		}

		/// <summary>
		/// The name of the process in which the activity should run. Normally, all components of an application run in a default process name created for the application and you do not need to use this attribute.
		/// But if necessary, you can override the default process name with this attribute, allowing you to spread your app components across multiple processes.
		/// </summary>
		/// <remarks>
		/// If the name assigned to this attribute begins with a colon (':'), a new process, private to the application, is created when it's needed and the activity runs in that process.
		/// If the process name begins with a lowercase character, the activity will run in a global process of that name, provided that it has permission to do so.
		/// This allows components in different applications to share a process, reducing resource usage.
		/// </remarks>
		public String Process
		{
			get
			{
				List<String> result = base.Node.GetAttribute("process");
				return result == null
					? base.ParentNode.Process
					: result[0];
			}
		}

		/// <summary>
		/// Whether or not the activity relinquishes its task identifiers to an activity above it in the task stack.
		/// A task whose root activity has this attribute set to "true" replaces the base Intent with that of the next activity in the task.
		/// If the next activity also has this attribute set to "true" then it will yield the base Intent to any activity that it launches in the same task.
		/// This continues for each activity until an activity is encountered which has this attribute set to "false"
		/// </summary>
		/// <remarks>This attribute set to "true" also permits the activity's use of the ActivityManager.TaskDescription to change labels, colors and icons in the overview screen.</remarks>
		[DefaultValue(false)]
		public Boolean RelinquishTaskIdentity
			=> base.GetBooleanAttribute("relinquishTaskIdentity").GetValueOrDefault(false);

		/// <summary>Specifies whether the app supports multi-window display.</summary>
		/// <remarks>
		/// If you set this attribute to true, the user can launch the activity in split-screen and freeform modes.
		/// If you set the attribute to false, the activity does not support multi-window mode.
		/// If this value is false, and the user attempts to launch the activity in multi-window mode, the activity takes over the full screen.
		/// 
		/// You can set this attribute in either the <see cref="ApkActivity"/> or <see cref="ApkApplication"/> element.
		/// </remarks>
		public Boolean? ResizeableActivity
			=> base.GetBooleanAttribute("resizeableActivity");

		/// <summary>The orientation of the activity's display on the device. The system ignores this attribute if the activity is running in multi-window mode.</summary>
		public ScreenOrientationType ScreenOrientation
		{
			get
			{
				List<String> result = base.Node.GetAttribute("screenOrientation");
				return result == null
					? ScreenOrientationType.unspecified
					: (ScreenOrientationType)Enum.Parse(typeof(ScreenOrientationType), result[0]);
			}
		}

		/// <summary>Whether or not the activity is shown when the device's current user is different than the user who launched the activity</summary>
		/// <remarks>This attribute was added in API level 23.</remarks>
		public Boolean? ShowForAllUsers
		{
			get
			{
				List<String> result = base.Node.GetAttribute("showForAllUsers");
				switch(result?[0])
				{
				case null:
					return (Boolean?)null;
				case "true":
					return true;
				case "false":
					return false;
				default:
					return base.GetResource(Convert.ToInt32(result[0])).Value == "true";
				}
			}
		}

		/// <summary>Whether or not the activity can be killed and successfully restarted without having saved its state — "true" if it can be restarted without reference to its previous state, and "false" if its previous state is required</summary>
		/// <remarks>
		/// Normally, before an activity is temporarily shut down to save resources, its onSaveInstanceState() method is called.
		/// This method stores the current state of the activity in a Bundle object, which is then passed to onCreate() when the activity is restarted.
		/// If this attribute is set to "true", onSaveInstanceState() may not be called and onCreate() will be passed null instead of the Bundle — just as it was when the activity started for the first time.
		/// 
		/// A "true" setting ensures that the activity can be restarted in the absence of retained state.
		/// For example, the activity that displays the home screen uses this setting to make sure that it does not get removed if it crashes for some reason.
		/// </remarks>
		[DefaultValue(false)]
		public Boolean StateNotNeeded
			=> base.GetBooleanAttribute("stateNotNeeded").GetValueOrDefault(false);

		/// <summary>Specifies whether the activity supports Picture-in-Picture display</summary>
		/// <remarks>This attribute was added in API level 24</remarks>
		public Boolean? SupportsPictureInPicture
			=> base.GetBooleanAttribute("supportsPictureInPicture");

		/// <summary>
		/// The task that the activity has an affinity for.
		/// Activities with the same affinity conceptually belong to the same task (to the same "application" from the user's perspective).
		/// The affinity of a task is determined by the affinity of its root activity.
		/// 
		/// The affinity determines two things — the task that the activity is re-parented to (see the allowTaskReparenting attribute) and the task that will house the activity when it is launched with the FLAG_ACTIVITY_NEW_TASK flag.
		/// </summary>
		/// <remarks>
		/// By default, all activities in an application have the same affinity.
		/// You can set this attribute to group them differently, and even place activities defined in different applications within the same task.
		/// To specify that the activity does not have an affinity for any task, set it to an empty string.
		/// </remarks>
		public String TaskAffinity
		{
			get
			{
				List<String> result = base.Node.GetAttribute("taskAffinity");
				return result == null
					? base.ParentNode.TaskAffinity
					: result[0];
			}
		}

		/// <summary>This automatically sets the activity's context to use this theme (see setTheme(), and may also cause "starting" animations prior to the activity being launched (to better match what the activity actually looks like).</summary>
		/// <remarks>
		/// If this attribute is not set, the activity inherits the theme set for the application as a whole — from the <see cref="ApkApplication"/> element's theme attribute.
		/// If that attribute is also not set, the default system theme is used.
		/// </remarks>
		public String Theme
		{
			get
			{
				List<String> result = base.Node.GetAttribute("theme");
				if(result == null)
					return base.ParentNode.Theme;

				Arsc.ResourceRow resourceRow = base.GetResource(Convert.ToInt32(result[0]));
				if(resourceRow != null)
					return resourceRow.Value;
				else
					return result[0];
			}
		}

		/// <summary>Extra options for an activity's UI</summary>
		/// <remarks>
		/// For more information about the app bar, see the Adding the App Bar training class.
		/// This attribute was added in API level 14
		/// </remarks>
		public uiOptonsType UIOptions
		{
			get
			{
				List<String> result = base.Node.GetAttribute("uiOptions");
				return result == null
					? uiOptonsType.none
					: (uiOptonsType)Enum.Parse(typeof(uiOptonsType), result[0]);
			}
		}

		/// <summary>How the main window of the activity interacts with the window containing the on-screen soft keyboard</summary>
		/// <remarks>
		/// The setting for this attribute affects two things:
		/// The state of the soft keyboard — whether it is hidden or visible — when the activity becomes the focus of user attention.
		/// The adjustment made to the activity's main window — whether it is resized smaller to make room for the soft keyboard or whether its contents pan to make the current focus visible when part of the window is covered by the soft keyboard.
		/// </remarks>
		public WindowSoftInputModeType[] WindowSoftInputMode
		{
			get
			{
				List<String> result = base.Node.GetAttribute("windowSoftInputMode");
				return result == null
					? new WindowSoftInputModeType[] { WindowSoftInputModeType.stateUnspecified }
					: Array.ConvertAll(result[0].Split('|'), delegate(String item) { return (WindowSoftInputModeType)Enum.Parse(typeof(WindowSoftInputModeType), result[0]); });
			}
		}

		/// <summary>Specifies the types of intents that an activity, service, or broadcast receiver can respond to</summary>
		/// <remarks>
		/// An intent filter declares the capabilities of its parent component — what an activity or service can do and what types of broadcasts a receiver can handle.
		/// It opens the component to receiving intents of the advertised type, while filtering out those that are not meaningful for the component.
		/// </remarks>
		public IEnumerable<ApkIntentFilter> IntentFilter
		{
			get
			{
				foreach(XmlNode node in base.Node["intent-filter"])
					yield return new ApkIntentFilter(this, node);
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


		internal ApkActivity(ApkApplication application, XmlNode node)
			: base(application, node)
		{ }
	}
}