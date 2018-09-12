
using Android.App;
using Android.OS;
using Android.Webkit;
using Com.Wikitude.Architect;

namespace XamarinExampleApp.Droid
{
    /*
     * This Activity is the least amount of code required to use the
     * basic functionality for Image-/Instant- and Object Tracking with the
     * addition of some code that is only needed for the example app to load the
     * right Experience with its configuration.
     *
     * This Activity needs Manifest.permission.CAMERA permissions granted before 
     * creating it because the ArchitectView will try to start the camera.
     */
    [Activity(Label = "SimpleArActivity", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class SimpleArActivity : Activity
    {
        // This key is used to send and retrieve the Experience configuration for the example app.
        public readonly static string IntentExtrasKeyExperienceData = "ExperienceData";

        private static readonly string ExperienceRootDir = "samples/";
        /*
         * The ArchitectView is the core of the AR functionality, it is the main
         * interface to the Wikitude SDK.
         * The ArchitectView has its own lifecycle which is very similar to the
         * Activity lifecycle.
         * To ensure that the ArchitectView is functioning properly the following
         * methods have to be called:
         *      - OnCreate(ArchitectStartupConfiguration)
         *      - OnPostCreate()
         *      - OnResume()
         *      - OnPause()
         *      - OnDestroy()
         * Those methods are preferably called in the corresponding Activity lifecycle callbacks.
         */
        protected ArchitectView architectView;

        /* The path to the AR-Experience.This is usually the path to its index.html. */
        private string arExperiencePath;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            /* 
             * Used to enabled remote debugging of the ArExperience with 
             * google chrome https://developers.google.com/web/tools/chrome-devtools/remote-debugging
             */
            WebView.SetWebContentsDebuggingEnabled(true);


            if (!Intent.HasExtra(IntentExtrasKeyExperienceData))
            {
                var message = GetString(Resource.String.error_loading_ar_experience_invalid_intent, Class.SimpleName, IntentExtrasKeyExperienceData);
                throw new System.Exception(message);
            }

            /*
             * The following code is used to run different configurations of the SimpleArActivity.
             * It is not required to use the ArchitectView but is used to simplify the example app.
             * 
             * Because of this the Activity has to be startet with correct intent extras.
             * e.g.:
             * var experience = new ArExperience(
             *      "ExperienceName", 
             *      "ExperiencePath", 
             *      Features.ImageTracking | Features.Geo, 
             *      CameraPosition.Back, 
             *      CameraResolution.Auto,
             *      CameraFocusMode.Continuous, 
             *      camera2Enabled: true
             * );
             * 
             * var intent = new Intent(this, typeof(SimpleArActivity));
             * intent.PutExtra(SimpleArActivity.IntentExtrasKeyExperienceData, ArExperience.Serialize(lastExperience));
             * StartActivity(intent);
             */
            var experience = ArExperience.Deserialize(Intent.GetByteArrayExtra(IntentExtrasKeyExperienceData));
            arExperiencePath = experience.Path;

            /*
             * The ArchitectStartupConfiguration is a required parameter for architectView.OnCreate.
             * It controls the startup of the ArchitectView which includes camera settings,
             * the required device features to run the ArchitectView and the LicenseKey which
             * has to be set to enable an AR-Experience.
             */
            var config = new ArchitectStartupConfiguration // Creates a config with its default values.
            {
                LicenseKey = GetString(Resource.String.wikitude_license_key), // Has to be set, to get a trial license key visit http://www.wikitude.com/developer/licenses.
                CameraPosition = Util.PlatformConverter.ConvertSharedToPlatformPosition(experience.CameraPosition), // The default camera is the first camera available for the system.
                CameraResolution = Util.PlatformConverter.ConvertSharedToPlatformResolution(experience.CameraResolution), // The default resolution is 640x480.
                CameraFocusMode = Util.PlatformConverter.ConvertSharedToPlatformFocusMode(experience.CameraFocusMode), // The default focus mode is continuous auto focus.
                Camera2Enabled = experience.Camera2Enabled, // The camera2 api is enabled by default on devices that support it.
                ArFeatures = (int)experience.FeaturesMask // This tells the ArchitectView which AR-features it is going to use, the default is all of them.
            };

            architectView = new ArchitectView(this);
            architectView.OnCreate(config);  // Mandatory ArchitectView lifecycle call

            SetContentView(architectView); // Adds the architectView to the activity.
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            architectView.OnPostCreate();  // Mandatory ArchitectView lifecycle call
            /*
             * Loads the AR-Experience, it may be a relative path from assets,
             * an absolute path (file://) or a server url.
             *
             * To get notified once the AR-Experience is fully loaded,
             * an ArchitectWorldLoadedListener can be registered.
             */
            architectView.Load(ExperienceRootDir + arExperiencePath);
        }

        protected override void OnResume()
        {
            base.OnResume();
            architectView.OnResume(); // Mandatory ArchitectView lifecycle call
        }

        protected override void OnPause()
        {
            base.OnPause();
            architectView.OnPause(); // Mandatory ArchitectView lifecycle call
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            /*
             * Deletes all cached files of this instance of the ArchitectView.
             * This guarantees that internal storage for this instance of the ArchitectView
             * is cleaned and app-memory does not grow each session.
             *
             * This should be called before architectView.onDestroy
             */
            architectView.ClearCache();
            architectView.OnDestroy(); // Mandatory ArchitectView lifecycle call
        }
    }
}
