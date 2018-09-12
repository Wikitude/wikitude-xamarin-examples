
using Android.App;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Com.Wikitude.Architect;

namespace XamarinExampleApp.Droid
{
    /*
     * This Activity is the least amount of code required to use the
     * basic functionality for Geo AR with the addition of some code that is 
     * only needed for the example app to load the right Experience with its configuration.
     *
     * This Activity needs Manifest.permission.ACCESS_FINE_LOCATION permissions
     * in addition to the required permissions of the SimpleArActivity.
     */
    [Activity(Label = "SimpleGeoArActivity", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class SimpleGeoArActivity : SimpleArActivity, ILocationListener, ArchitectView.ISensorAccuracyChangeListener
    {
        /*
         * Very basic location provider to enable location updates.
         * Please note that this approach is very minimal and we recommend to implement a more
         * advanced location provider for your app. (see https://developer.android.com/training/location/index.html)
         */
        private Util.location.LocationProvider locationProvider;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            locationProvider = new Util.location.LocationProvider(this, this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (!locationProvider.Start())
            {
                Toast.MakeText(this, Resource.String.no_location_provider, ToastLength.Long).Show();
            }
            /*
             * The SensorAccuracyChangeListener has to be registered to the Architect view after ArchitectView.OnCreate.
             * There may be more than one SensorAccuracyChangeListener.
             */
            architectView.RegisterSensorAccuracyChangeListener(this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            locationProvider.Stop();
            // The SensorAccuracyChangeListener has to be unregistered from the Architect view before ArchitectView.onDestroy.
            architectView.UnregisterSensorAccuracyChangeListener(this);
        }

        /*
         * The ArchitectView has to be notified when the location of the device
         * changed in order to accurately display the Augmentations for Geo AR.
         *
         * The ArchitectView has two methods which can be used to pass the Location,
         * it should be chosen by whether an altitude is available or not.
         */
        public virtual void OnLocationChanged(Location location)
        {
            float accuracy = location.HasAccuracy ? location.Accuracy : 1000;
            if (location.HasAltitude)
            {
                architectView.SetLocation(location.Latitude, location.Longitude, location.Altitude, accuracy);
            }
            else
            {
                architectView.SetLocation(location.Latitude, location.Longitude, accuracy);
            }
        }

        /*
         * The very basic Activity setup of this sample app does not handle the following callbacks
         * to keep the sample app as small as possible. They should be used to handle changes in a production app.
         */
        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras) { }


        /*
         * The ArchitectView.ISensorAccuracyChangeListener notifies of changes in the accuracy of the compass.
         * This can be used to notify the user that the sensors need to be recalibrated.
         *
         * This listener has to be registered after OnCreate and unregistered before OnDestroy in the ArchitectView.
         */
        public void OnCompassAccuracyChanged(int accuracy)
        {
            if (accuracy < 2)
            { // UNRELIABLE = 0, LOW = 1, MEDIUM = 2, HIGH = 3
                Toast.MakeText(this, Resource.String.compass_accuracy_low, ToastLength.Long).Show();
            }
        }
    }
}
