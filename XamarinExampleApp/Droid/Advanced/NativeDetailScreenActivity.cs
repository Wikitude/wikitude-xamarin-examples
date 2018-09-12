
using Android.App;
using Android.Content;
using Android.OS;
using Com.Wikitude.Architect;
using Org.Json;

namespace XamarinExampleApp.Droid.Advanced
{
    /*
     * This Activity is used for the NativeDetailScreen ArExperience.
     * It is used to start a new Activity that displays the details of the clicked POI.
     *
     * For this functionality it implements an ArchitectJavaScriptInterfaceListener.
     */
    [Activity(Label = "NativeDetailScreenActivity", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class NativeDetailScreenActivity : SimpleGeoArActivity, IArchitectJavaScriptInterfaceListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            /*
             * The ArchitectJavaScriptInterfaceListener has to be added to the Architect view after ArchitectView.OnCreate.
             * There may be more than one ArchitectJavaScriptInterfaceListener.
             */
            architectView.AddArchitectJavaScriptInterfaceListener(this);
        }

        protected override void OnDestroy()
        {
            // The ArchitectJavaScriptInterfaceListener has to be removed from the Architect view before ArchitectView.OnDestroy.
            architectView.RemoveArchitectJavaScriptInterfaceListener(this);
            base.OnDestroy();
        }

        public void OnJSONObjectReceived(JSONObject jsonObject)
        {
            if (jsonObject.GetString("action") == "present_poi_details")
            {
                Intent poiDetailIntent = new Intent(this, typeof(SamplePoiDetailActivity));
                poiDetailIntent.PutExtra(SamplePoiDetailActivity.extrasKeyPoiId, jsonObject.GetString("id"));
                poiDetailIntent.PutExtra(SamplePoiDetailActivity.extrasKeyPoiTitle, jsonObject.GetString("title"));
                poiDetailIntent.PutExtra(SamplePoiDetailActivity.extrasKeyPoiDescription, jsonObject.GetString("description"));
                StartActivity(poiDetailIntent);
            }
        }
    }
}
