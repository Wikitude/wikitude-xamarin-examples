using System;
using System.Json;
using CoreLocation;

namespace XamarinExampleApp.iOS.AdvancedViewControllers
{
    public partial class ObtainPoiDataFromApplicationModelViewController : ArExperienceViewController
    {
        protected CLLocationManager locationManager;

        public ObtainPoiDataFromApplicationModelViewController(ArExperience arExperience) : base(arExperience)
        {
            locationManager = new CLLocationManager();
            locationManager.LocationsUpdated += LocationsUpdated;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            locationManager.StartUpdatingLocation();
        }

        private void LocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
        {
            if (e.Locations.Length > 0) {
                JsonArray jsonArray = PoiGenerator.GeneratePoiInformation(e.Locations[0].Coordinate.Latitude, e.Locations[0].Coordinate.Longitude);
                architectView.CallJavaScript("World.loadPoisFromJsonData(" + jsonArray.ToString() + ")"); // Triggers the loadPoisFromJsonData function

                locationManager.StopUpdatingLocation();
                locationManager.LocationsUpdated -= LocationsUpdated;
            }
        }
    }
}

