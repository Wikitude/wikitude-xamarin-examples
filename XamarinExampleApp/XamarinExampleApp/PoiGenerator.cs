using System;
using System.Json;
using System.Collections.Generic;

namespace XamarinExampleApp
{
    public class PoiGenerator
    {
        public PoiGenerator()
        {
        }

        public static JsonArray GeneratePoiInformation(double latitude, double longitude)
        {
            JsonValue[] pois = new JsonValue[20];

            // ensure these attributes are also used in JavaScript when extracting POI data
            string attrId = "id";
            string attrName = "name";
            string attrDescription = "description";
            string attrLatitude = "latitude";
            string attrLongitude = "longitude";
            string attrAltitude = "altitude";

            Random random = new Random();
            // generates 20 POIs
            for (int i = 0; i < 20; i++)
            {
                double[] poiLocationLatLon = GetRandomLatLonNearby(random, latitude, longitude);
                float unknownAltitude = -32768f;  // equals "AR.CONST.UNKNOWN_ALTITUDE" in JavaScript (compare AR.GeoLocation specification)
                // Use "AR.CONST.UNKNOWN_ALTITUDE" to tell ARchitect that altitude of places should be on user level. Be aware to handle altitude properly in locationManager in case you use valid POI altitude value (e.g. pass altitude only if GPS accuracy is <7m).

                var id = i + 1;
                var poiInformation = new Dictionary<string, JsonValue>
                {
                    { attrId, id },
                    { attrName, "POI#" + id },
                    { attrDescription, "This is the description of POI#" + id },
                    { attrLatitude, poiLocationLatLon[0] },
                    { attrLongitude, poiLocationLatLon[1] },
                    { attrAltitude, unknownAltitude }
                };
                pois[i] = new JsonObject(poiInformation);
            }

            return new JsonArray(pois);
        }

        private static double[] GetRandomLatLonNearby(Random random, double latitude, double longitude)
        {
            double minimumOffset = -0.3;
            double maximumOffset = 0.3;

            double latitudeOffset = random.NextDouble() * (maximumOffset - minimumOffset) + minimumOffset;
            double longitudeOffset = random.NextDouble() * (maximumOffset - minimumOffset) + minimumOffset;
            return new double[] { latitude + latitudeOffset, longitude + longitudeOffset };
        }
    }
}
