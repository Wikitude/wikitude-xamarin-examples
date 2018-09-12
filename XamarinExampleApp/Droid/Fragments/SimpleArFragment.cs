
using Android.OS;
using Android.Views;
using Android.Support.V4.App;
using Com.Wikitude.Architect;
using Android.Webkit;

namespace XamarinExampleApp.Droid.Fragments
{
    public class SimpleArFragment : Fragment
    {
        public readonly static string IntentExtrasKeyExperienceData = "ExperienceData";
        protected ArchitectView architectView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            WebView.SetWebContentsDebuggingEnabled(true);

            architectView = new ArchitectView(Context);
            return architectView;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            var experience = ArExperience.Deserialize(Arguments.GetByteArray(IntentExtrasKeyExperienceData));

            var arExperiencePath = experience.Path;

            var config = new ArchitectStartupConfiguration
            {
                LicenseKey = GetString(Resource.String.wikitude_license_key),
                CameraPosition = Util.PlatformConverter.ConvertSharedToPlatformPosition(experience.CameraPosition),
                CameraResolution = Util.PlatformConverter.ConvertSharedToPlatformResolution(experience.CameraResolution),
                CameraFocusMode = Util.PlatformConverter.ConvertSharedToPlatformFocusMode(experience.CameraFocusMode),
                Camera2Enabled = experience.Camera2Enabled,
                ArFeatures = (int)experience.FeaturesMask
            };

            architectView.OnCreate(config);
            architectView.OnPostCreate();

            architectView.Load(arExperiencePath);
        }

        public override void OnResume()
        {
            base.OnResume();
            architectView.OnResume();
        }

        public override void OnPause()
        {
            base.OnPause();
            architectView.OnPause();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            architectView.ClearCache();
            architectView.OnDestroy();
        }
    }
}
