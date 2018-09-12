using System.Collections.Generic;
using Android;
using Android.Content;
using Android.Locations;
using Android.Support.V4.Content;
using Android.Util;

namespace XamarinExampleApp.Droid.Util.location
{
    public class LocationProvider
    {
        private static readonly int LocationUpdateMinTime = 1000;
        private static readonly int LocationUpdateDistance = 0;
        private static readonly int LocationOutdatedWhenOlderMs = 1000 * 60 * 10;

        private readonly Context context;
        private readonly ILocationListener listener;

        private readonly LocationManager locationManager;

        public LocationProvider(Context context, ILocationListener listener)
        {
            this.context = context;
            this.listener = listener;

            locationManager = context.GetSystemService(Context.LocationService) as LocationManager;                     
        }

        public bool Start() 
        {
            if (ContextCompat.CheckSelfPermission(context, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted)
            {
                Log.Error("LocationProvider", context.GetString(Resource.String.permissions_denied) + " Manifest.Permission.AccessFineLocation");
                return false;
            }

            var gpsProviderEnabled = locationManager?.IsProviderEnabled(LocationManager.GpsProvider) ?? false;
            var networkProviderEnabled = locationManager?.IsProviderEnabled(LocationManager.NetworkProvider) ?? false;

            var providers = new List<string>();

            if (gpsProviderEnabled)
            {
                providers.Add(LocationManager.GpsProvider);
            }
            if (networkProviderEnabled)
            {
                providers.Add(LocationManager.NetworkProvider);
            }

            foreach (var provider in providers) 
            {
                var lastKnownLocation = locationManager.GetLastKnownLocation(provider);
                if (lastKnownLocation != null && lastKnownLocation.Time > Java.Lang.JavaSystem.CurrentTimeMillis() - LocationOutdatedWhenOlderMs)
                {
                    listener.OnLocationChanged(lastKnownLocation);
                }
                if (locationManager.GetProvider(provider) != null)
                {
                    locationManager.RequestLocationUpdates(provider, LocationUpdateMinTime, LocationUpdateDistance, listener);
                }
            }

            if (!gpsProviderEnabled && !networkProviderEnabled)
            {
                return false;    
            }
            return true;
        }

        public void Stop()
        {
            locationManager.RemoveUpdates(listener);
        }
    }
}
