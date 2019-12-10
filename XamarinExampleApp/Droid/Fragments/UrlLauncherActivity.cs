
using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;

namespace XamarinExampleApp.Droid.Fragments
{
    [Activity(Label = "UrlLauncherActivity")]
    public class UrlLauncherActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Activity_url_launcher);

            var experienceBytes = Intent.GetByteArrayExtra(SimpleArFragment.IntentExtrasKeyExperienceData);
            var experience = ArExperience.Deserialize(experienceBytes);

            Android.Support.V4.App.Fragment fragment = (experience.FeaturesMask & Features.Geo) == Features.Geo ? new SimpleGeoFragment() : new SimpleArFragment();

            var args = new Bundle();
            args.PutByteArray(SimpleArFragment.IntentExtrasKeyExperienceData, experienceBytes);
            fragment.Arguments = args;

            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.mainFragement, fragment);
            fragmentTransaction.Commit();

            // Prevent device from sleeping
            Window.AddFlags(flags: WindowManagerFlags.KeepScreenOn);
        }
    }
}
