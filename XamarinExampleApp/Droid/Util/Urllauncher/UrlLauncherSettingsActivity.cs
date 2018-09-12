using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace XamarinExampleApp.Droid.Util.Urllauncher
{
    [Activity(Label = "UrlLauncherSettingsActivity")]
    public class UrlLauncherSettingsActivity : Activity
    {
        private static readonly string AutocompletePrefs = "autocomplete_prefs";
        private static readonly string AutocompletePrefsHistory = "autocomplete_prefs_history";

        private ICollection<string> history;

        private ArExperienceGroup experienceGroup;
        private int editPosition;
        private AutoCompleteTextView urlView;
        private EditText nameView;
        private Switch geoSwitch;
        private Switch imageSwitch;
        private Switch instantSwitch;
        private Switch objectSwitch;
        private Switch camera2Switch;
        private Spinner focusSpinner;
        private Spinner positionSpinner;
        private Spinner resolutionSpinner;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Url_launcher_row_settings);

            var preferences = GetSharedPreferences(AutocompletePrefs, 0);
            history = preferences.GetStringSet(AutocompletePrefsHistory, new List<string>());
            UpdateAutocompleteSource();

            urlView = FindViewById(Resource.Id.url_launcher_url_text) as AutoCompleteTextView;
            nameView = FindViewById(Resource.Id.url_launcher_name_edit_text) as EditText;
            geoSwitch = FindViewById(Resource.Id.url_launcher_geo_switch) as Switch;
            imageSwitch = FindViewById(Resource.Id.url_launcher_image_switch) as Switch;
            instantSwitch = FindViewById(Resource.Id.url_launcher_instant_switch) as Switch;
            objectSwitch = FindViewById(Resource.Id.url_launcher_object_switch) as Switch;
            camera2Switch = FindViewById(Resource.Id.url_launcher_camera2_switch) as Switch;
            focusSpinner = FindViewById(Resource.Id.url_launcher_focus_spinner) as Spinner;
            positionSpinner = FindViewById(Resource.Id.url_launcher_position_spinner) as Spinner;
            resolutionSpinner = FindViewById(Resource.Id.url_launcher_resolution_spinner) as Spinner;

            editPosition = Intent.GetIntExtra(UrlLauncherStorageActivity.UrlLauncherEditExperienceId, -1);
            experienceGroup = ArExperienceGroup.Deserialize(Intent.GetByteArrayExtra(UrlLauncherStorageActivity.UrlLauncherExperienceGroup));

            if (editPosition >= 0)
            {
                var experience = experienceGroup.ArExperiences[editPosition];
                urlView.Text = experience.Path;
                nameView.Text = experience.Name;

                var features = experience.FeaturesMask;
                geoSwitch.Checked = (features & Features.Geo) == Features.Geo;
                imageSwitch.Checked = (features & Features.ImageTracking) == Features.ImageTracking;
                instantSwitch.Checked = (features & Features.InstantTracking) == Features.InstantTracking;
                objectSwitch.Checked = (features & Features.ObjectTracking) == Features.ObjectTracking;

                int position;
                switch (experience.CameraPosition)
                {
                    case CameraPosition.Back: position = 0; break;
                    case CameraPosition.Front: position = 1; break;
                    default: position = 0; break;
                }
                positionSpinner.SetSelection(position);

                int resolution;
                switch (experience.CameraResolution)
                {
                    case CameraResolution.HD_1280x720: resolution = 1; break;
                    case CameraResolution.Full_HD_1920x1080: resolution = 2; break;
                    case CameraResolution.Auto: resolution = 3; break;
                    default: resolution = 0; break;
                }
                resolutionSpinner.SetSelection(resolution);

                int focusMode;
                switch (experience.CameraFocusMode)
                {
                    case CameraFocusMode.AutofocusOnce: focusMode = 1; break;
                    case CameraFocusMode.AutofocusOff: focusMode = 2; break;
                    default: focusMode = 0; break;
                }
                focusSpinner.SetSelection(focusMode);

                camera2Switch.Checked = experience.Camera2Enabled;
            }

            urlView.EditorAction += (sender, args) =>
            {
                if (args.ActionId == ImeAction.Send)
                {
                    StoreSettings();
                    args.Handled = true;
                }
            };

            var ok = FindViewById(Resource.Id.url_launcher_ok_button) as Button;
            ok.Click += (sender, args) =>
            {
                StoreSettings();
            };
        }


        private void StoreSettings()
        {
            history.Add(urlView.Text);
            UpdateAutocompleteSource();
            var preferences = GetSharedPreferences(AutocompletePrefs, 0);
            var editor = preferences.Edit().PutStringSet(AutocompletePrefsHistory, history);
            editor.Apply();

            Features features = 0;
            if (geoSwitch.Checked) {
                features |= Features.Geo;
            }
            if (imageSwitch.Checked)
            {
                features |= Features.ImageTracking;
            }
            if (instantSwitch.Checked)
            {
                features |= Features.InstantTracking;
            }
            if (objectSwitch.Checked)
            {
                features |= Features.ObjectTracking;
            }

            CameraPosition cameraPosition;
            var position = positionSpinner.SelectedItemId;
            switch (position)
            {
                case 1: cameraPosition = CameraPosition.Front; break;
                default: cameraPosition = CameraPosition.Back; break;
            }
            CameraResolution cameraResolution;
            var resolution = resolutionSpinner.SelectedItemId;
            switch (resolution)
            {
                case 1: cameraResolution = CameraResolution.HD_1280x720; break;
                case 2: cameraResolution = CameraResolution.Full_HD_1920x1080; break;
                case 3: cameraResolution = CameraResolution.Auto; break;
                default: cameraResolution = CameraResolution.SD_640x480; break;
            }
            CameraFocusMode cameraFocusMode;
            switch(focusSpinner.SelectedItemId)
            {
                case 1: cameraFocusMode = CameraFocusMode.AutofocusOnce; break;
                case 2: cameraFocusMode = CameraFocusMode.AutofocusOff; break;
                default: cameraFocusMode = CameraFocusMode.AutofocusContinuous; break;
            }

            var experience = new ArExperience(nameView.Text, urlView.Text, features, cameraPosition, cameraResolution, cameraFocusMode, camera2Enabled: camera2Switch.Checked);
            if (editPosition >= 0)
            {
                experienceGroup.ArExperiences[editPosition] = experience;
            } 
            else 
            {
                experienceGroup.ArExperiences.Add(experience);
            }

            var intent = new Intent(this, typeof(UrlLauncherStorageActivity));
            intent.PutExtra(UrlLauncherStorageActivity.UrlLauncherExperienceGroup, ArExperienceGroup.Serialize(experienceGroup));
            intent.AddFlags(ActivityFlags.ReorderToFront);
            StartActivity(intent);
        }

        private void UpdateAutocompleteSource()
        {
            var urlText = FindViewById(Resource.Id.url_launcher_url_text) as AutoCompleteTextView;
            ArrayAdapter adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, new List<string>(history));
            urlText.Adapter = adapter;
        }
    }
}
