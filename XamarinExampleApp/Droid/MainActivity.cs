using Android.App;
using Android.OS;
using System.IO;
using Com.Wikitude.Architect;
using Android.Support.V7.App;
using Android.Widget;
using System.Collections.Generic;
using Android.Content;
using Android.Util;
using Android.Views;
using Android;
using Com.Wikitude.Common;
using Com.Wikitude.Common.Devicesupport;
using Com.Wikitude.Common.Permission;
using XamarinExampleApp.Droid.Advanced;
using System;
using XamarinExampleApp.Droid.Util.Urllauncher;
using Java.Util;

namespace XamarinExampleApp.Droid
{
    /*
    * The MainActivity is used to display the list of all ArExperiences and handles the runtime
    * permissions for them.
    */
    [Activity(Label = "Xamarin Example App", MainLauncher = true, Icon = "@mipmap/ic_launcher", RoundIcon = "@mipmap/ic_launcher_round")]
    public class MainActivity : AppCompatActivity, IPermissionManagerPermissionManagerCallback
    {
        private ExpandableListView listView;
        private List<ArExperienceGroup> experienceGroups;
        private ArExperience lastExperience;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Activity_Main);

            /*
             * Loads the experience definiton file from the assets and parses the content to 
             * ArExperienceGroups. 
             */
            var stream = Assets.Open("samples/samples.json");
            StreamReader sr = new StreamReader(stream);
            experienceGroups = ArExperienceManager.ParseExampleDefintion(sr.ReadToEnd());
            sr.Close();

            /*
             * Removes all ArExperiences that are not supported on the current device because
             * of missing hardware requirements like sensors.
             */
            int removedExperiences = FilterUnsupportedArExperiences(this, experienceGroups);
            if (removedExperiences > 0)
            {
                string message = GetString(Resource.String.error_loading_ar_experience_unsupported, removedExperiences);
                Toast.MakeText(this, message, ToastLength.Long).Show();
            }

            /*
             * Fills the ExpandableListView with all remaining ArExperiences.
             */
            var adapter = new Util.adapters.ArExpandableListAdapter(this, experienceGroups);
            listView = FindViewById(Resource.Id.listView) as ExpandableListView;
            MoveExpandableIndicatorToRight();
            listView.ChildClick += ExperienceListChildClick;
            listView.SetAdapter(adapter);

            /*
             * Sets the title and the menu of the Toolbar. The menu contains two items
             * one for laoding a custom ArExperience from a URL and one for showing informations about 
             * the Version of the Wikitude SDK.
             */
            var toolbar = FindViewById(Resource.Id.toolbar) as Android.Support.V7.Widget.Toolbar;
            toolbar.InflateMenu(Resource.Menu.main_menu);
            toolbar.MenuItemClick += (sender, args) => {
                switch (args.Item.ItemId) 
                {
                    case Resource.Id.menu_main_load_url:
                        var intent = new Intent(this, typeof(UrlLauncherStorageActivity));
                        StartActivity(intent);
                        args.Handled = true;
                        break;
                    case Resource.Id.menu_main_info:
                        var sdkBuildInformation = ArchitectView.SDKBuildInformation;
                        new Android.Support.V7.App.AlertDialog.Builder(this)
                                .SetTitle(Resource.String.build_information_title)
                                .SetMessage(
                                           GetString(Resource.String.build_information_config) + sdkBuildInformation.BuildConfiguration + "\n" +
                                           GetString(Resource.String.build_information_date) + sdkBuildInformation.BuildDate + "\n" +
                                           GetString(Resource.String.build_information_number) + sdkBuildInformation.BuildNumber + "\n" +
                                           GetString(Resource.String.build_information_version) + ArchitectView.SDKVersion)
                               .Show();
                        args.Handled = true;
                        break;
                    default:
                        args.Handled = false;
                        break;
                }
            };
            toolbar.SetTitle(Resource.String.app_name);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            int[] results = new int[grantResults.Length];
            for (int i = 0; i < grantResults.Length; i++) 
            {
                results[i] = (int)grantResults[i];
            }
            ArchitectView.PermissionManager.OnRequestPermissionsResult(requestCode, permissions, results);
        }

        public void ExperienceListChildClick(object sender, ExpandableListView.ChildClickEventArgs args)
        {
            lastExperience = experienceGroups[args.GroupPosition].ArExperiences[args.ChildPosition];

            string[] permissions = (lastExperience.FeaturesMask & Features.Geo) == Features.Geo ?
                    new string[] { Manifest.Permission.Camera, Manifest.Permission.AccessFineLocation } :
                    new string[] { Manifest.Permission.Camera };

            ArchitectView.PermissionManager.CheckPermissions(this, permissions, PermissionManager.WikitudePermissionRequest, this);
            args.Handled = true;
        }

        public void PermissionsDenied(string[] deniedPermissions)
        {
            Toast.MakeText(this, GetString(Resource.String.permissions_denied) + string.Join(",",deniedPermissions), ToastLength.Short).Show();
        }

        public void PermissionsGranted(int responseCode)
        {
            /*
             * For ArExperiences that require an interaction of JS and Native code more 
             * advanced Activities will be loaded. 
             * For ArExperiences that don't require any interactions either the SimpleArActivity
             * or the SimpleGeoArActivity are loaded. Those activites contain the least amount of
             * code required to use the Wikitude SDK with the addition of loading the configuarion 
             * form Intent extras.
             */
            var activity = GetActivityForExperience(lastExperience);
            var intent = new Intent(this, activity);
            intent.PutExtra(SimpleArActivity.IntentExtrasKeyExperienceData, ArExperience.Serialize(lastExperience));
            StartActivity(intent);
        }

        public void ShowPermissionRationale(int requestCode, string[] permissions)
        {
            var alertBuilder = new Android.App.AlertDialog.Builder(this);
            alertBuilder.SetCancelable(true);
            alertBuilder.SetTitle(Resource.String.permission_rationale_title);
            alertBuilder.SetMessage(GetString(Resource.String.permission_rationale_text) + string.Join(",", permissions));

            alertBuilder.SetPositiveButton(Android.Resource.String.Yes, new EventHandler<DialogClickEventArgs>((sender, eventArgs) => {
                ArchitectView.PermissionManager.PositiveRationaleResult(requestCode, permissions);
            }));

            var alert = alertBuilder.Create();
            alert.Show();
        }

        private void MoveExpandableIndicatorToRight()
        {
            var metrics = new DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(metrics);

            int width = metrics.WidthPixels;

            listView?.SetIndicatorBoundsRelative(width - DpToPx(60), width - DpToPx(30));
            listView?.SetIndicatorBoundsRelative(width - DpToPx(60), width - DpToPx(30));
        }

        private int DpToPx(int dp)
        {
            float scale = Resources.DisplayMetrics.Density;
            return (int)(dp * scale + 0.5f);
        }

        private static int FilterUnsupportedArExperiences(Context context, List<ArExperienceGroup> experienceGroups) {
            int removedExperiences = 0;
            foreach (var group in experienceGroups) {
                removedExperiences += group.ArExperiences.RemoveAll(experience => !ArchitectView.IsDeviceSupporting(context, GetEnumSetFeatures(experience.FeaturesMask)).IsSuccess);
            }
            experienceGroups.RemoveAll(group => group.ArExperiences.Count == 0);
            return removedExperiences;
        }

        private static EnumSet GetEnumSetFeatures(Features features)
        {
            EnumSet featuresSet = EnumSet.Of(Feature.ImageTracking);
            if ((features & Features.ImageTracking) != Features.ImageTracking)
            {
                featuresSet.Remove(Feature.ImageTracking);
            }
            if ((features & Features.ObjectTracking) == Features.ObjectTracking)
            {
                featuresSet.Add(Feature.ObjectTracking);
            }
            if ((features & Features.InstantTracking) == Features.InstantTracking)
            {
                featuresSet.Add(Feature.InstantTracking);
            }
            if ((features & Features.Geo) == Features.Geo)
            {
                featuresSet.Add(Feature.Geo);
            }

            return featuresSet;
        }

        private Type GetActivityForExperience(ArExperience experience)
        {
            Type experienceActivity;
            switch (experience.Extension) 
            {
                case "Screenshot":
                    experienceActivity = typeof(ScreenshotActivity);
                    break;
                case "SaveAndLoadInstantTarget":
                    experienceActivity = typeof(SaveAndLoadInstantTargetActivity);
                    break;
                case "ObtainPoiDataFromApplicationModel":
                    experienceActivity = typeof(ObtainPoiDataFromApplicationModelActivity);
                    break;
                case "NativeDetailScreen":
                    experienceActivity = typeof(NativeDetailScreenActivity);
                    break;
                case "PoisCaptureScreen":
                    experienceActivity = typeof(PoisCaptureScreenActivity);
                    break;
                default:
                    experienceActivity = (experience.FeaturesMask & Features.Geo) == Features.Geo ? typeof(SimpleGeoArActivity) : typeof(SimpleArActivity);                           
                    break;
            }
            return experienceActivity;
        }
    }
}

