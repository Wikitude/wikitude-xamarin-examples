
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Webkit;
using Android.Widget;
using Com.Wikitude.Architect;
using System.Json;

namespace XamarinExampleApp.Droid.Advanced
{
    [Activity(Label = "ObtainPoiDataFromApplicationModelActivity", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ObtainPoiDataFromApplicationModelActivity : SimpleGeoArActivity
    {
        // If the POIs were already generated and sent to JavaScript.
        private bool injectedPois = false;

        public override void OnLocationChanged(Location location)
        {
            base.OnLocationChanged(location);
            /*
             * When the first location was received the POIs are generated and sent to the JavaScript code,
             * by using architectView.callJavascript.
             */
            if (!injectedPois)
            {
                JsonArray jsonArray = PoiGenerator.GeneratePoiInformation(location.Latitude, location.Longitude);
                architectView.CallJavascript("World.loadPoisFromJsonData(" + jsonArray.ToString() + ")"); // Triggers the loadPoisFromJsonData function
                injectedPois = true; // don't load pois again
            }
        }
    }
}
