
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Wikitude.Architect;
using Com.Wikitude.Common.Permission;
using XamarinExampleApp.Droid.Fragments;
using XamarinExampleApp.Droid.Util.Adapters;

namespace XamarinExampleApp.Droid.Util.Urllauncher
{
    [Activity(Label = "UrlLauncherStorageActivity")]
    public class UrlLauncherStorageActivity : AppCompatActivity, IPermissionManagerPermissionManagerCallback, IDialogInterfaceOnCancelListener
    {

        public static readonly string UrlLauncherExperienceGroup = "urlLauncherExperienceGroup";
        public static readonly string UrlLauncherEditExperienceId = "urlLauncherExperienceGroupId";

        private static readonly string StorageFile = "urlLauncherExperienceGroupStorage";

        private ArExperienceGroup experienceGroup = new ArExperienceGroup("", new List<ArExperience>());
        private ArExperience lastExperience;
        private UrlLauncherStorageListAdapter adapter;

        private bool startingAr = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Activity_url_launcher_storage);

            var toolbar = FindViewById(Resource.Id.toolbar) as Android.Support.V7.Widget.Toolbar;
            toolbar.SetTitle(Resource.String.url_launcher_title);

            SetSupportActionBar(toolbar);

            var addUrlFab = FindViewById(Resource.Id.url_add_experience_fab);
            addUrlFab.Click += (sender, e) => {
                var intent = new Intent(this, typeof(UrlLauncherSettingsActivity));
                intent.PutExtra(UrlLauncherExperienceGroup, ArExperienceGroup.Serialize(experienceGroup));
                StartActivity(intent);
            };
        }

        protected override void OnResume()
        {
            base.OnResume();
            startingAr = false;

            if (Intent.HasExtra(UrlLauncherExperienceGroup))
            {
                experienceGroup = ArExperienceGroup.Deserialize(Intent.GetByteArrayExtra(UrlLauncherExperienceGroup));
            }
            else 
            {
                try
                {
                    using (var stream = OpenFileInput(StorageFile))
                    {
                        var formatter = new BinaryFormatter();
                        experienceGroup = formatter.Deserialize(stream) as ArExperienceGroup;
                    }
                }
                catch (Java.IO.FileNotFoundException) { } 
            }

            var listView = FindViewById(Resource.Id.url_list_storage_view) as ListView;
            listView.ItemClick += (sender, e) => {
                if (!startingAr)
                {
                    startingAr = true;

                    lastExperience = experienceGroup.ArExperiences[e.Position];

                    string[] permissions = (lastExperience.FeaturesMask & Features.Geo) == Features.Geo ?
                            new string[] { Manifest.Permission.Camera, Manifest.Permission.AccessFineLocation } :
                            new string[] { Manifest.Permission.Camera };

                    ArchitectView.PermissionManager.CheckPermissions(this, permissions, PermissionManager.WikitudePermissionRequest, this);
                }
            };
            adapter = new UrlLauncherStorageListAdapter(this, experienceGroup);
            listView.Adapter = adapter;
        }

        protected override void OnPause()
        {
            base.OnPause();

            using (var stream = OpenFileOutput(StorageFile, FileCreationMode.Private))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, experienceGroup);
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            Intent = intent;
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

        public void PermissionsDenied(string[] deniedPermissions)
        {
            Toast.MakeText(this, GetString(Resource.String.permissions_denied) + string.Join(",", deniedPermissions), ToastLength.Short).Show();
            startingAr = false;
        }

        public void PermissionsGranted(int responseCode)
        {
            var intent = new Intent(this, typeof(UrlLauncherActivity));
            intent.PutExtra(SimpleArFragment.IntentExtrasKeyExperienceData, ArExperience.Serialize(lastExperience));
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
            alertBuilder.SetOnCancelListener(this);

            var alert = alertBuilder.Create();
            alert.Show();
        }


        public void OnCancel(IDialogInterface dialog)
        {
            startingAr = false;
        }
    }
}
