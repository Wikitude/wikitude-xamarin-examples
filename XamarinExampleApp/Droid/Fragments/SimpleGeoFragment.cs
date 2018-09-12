
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Com.Wikitude.Architect;

namespace XamarinExampleApp.Droid.Fragments
{
    public class SimpleGeoFragment : SimpleArFragment, ILocationListener, ArchitectView.ISensorAccuracyChangeListener
    {
        private Util.location.LocationProvider locationProvider;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            locationProvider = new Util.location.LocationProvider(Context, this);
        }
        public override void OnResume()
        {
            base.OnResume();
            if (!locationProvider.Start())
            {
                Toast.MakeText(Context, "Could not start Location updates. Make sure that locations and location providers are enabled and Runtime Permissions are granted.", ToastLength.Long).Show();
            }
            architectView.RegisterSensorAccuracyChangeListener(this);
        }

        public override void OnPause()
        {
            base.OnPause();
            locationProvider.Stop();
            architectView.UnregisterSensorAccuracyChangeListener(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void OnLocationChanged(Location location)
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

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras) { }

        public void OnCompassAccuracyChanged(int accuracy)
        {
            if (accuracy < 2)
            { // UNRELIABLE = 0, LOW = 1, MEDIUM = 2, HIGH = 3
                Toast.MakeText(Context, Resource.String.compass_accuracy_low, ToastLength.Long).Show();
            }
        }
    }
}
