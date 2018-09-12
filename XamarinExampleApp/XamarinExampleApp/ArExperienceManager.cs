using System.IO;
using System.Collections.Generic;
using System.Json;
using System.Runtime.Serialization.Formatters.Binary;

namespace XamarinExampleApp
{
    public static class ArExperienceManager
    {
        public static List<ArExperienceGroup> ParseExampleDefintion(string defintions)
        {
            var experienceGroups = new List<ArExperienceGroup>();

            JsonArray sampleGroups = (JsonArray)JsonValue.Parse(defintions);
            foreach (JsonObject sampleGroup in sampleGroups)
            {
                string groupName = sampleGroup["windowTitle"];
                JsonArray experiences = (JsonArray)sampleGroup["samples"];

                var arExperiences = new List<ArExperience>();

                foreach (JsonObject experience in experiences)
                {
                    string experienceName = experience["title"];
                    string experiencePath = experience["path"];
                    string experienceExtension = null;
                    if (experience.TryGetValue("requiredExtension", out JsonValue extension))
                    {
                        experienceExtension = extension;
                    }
                    JsonArray featuresArray = (JsonArray)experience["requiredFeatures"];
                    JsonObject startupConfig = (JsonObject)experience["startupConfiguration"];

                    Features experienceFeatures = ParseFeatures(featuresArray);
                    CameraPosition cameraPosition = ParseCameraPosition(startupConfig);
                    CameraResolution cameraResolution = ParseCameraResolution(startupConfig);

                    var arExperience = new ArExperience(experienceName, experiencePath, experienceFeatures, cameraPosition, cameraResolution, extension: experienceExtension);
                    arExperiences.Add(arExperience);
                }

                var arExperienceGroup = new ArExperienceGroup(groupName, arExperiences);
                experienceGroups.Add(arExperienceGroup);
            }


            return experienceGroups;
        }

        public static void WriteToFile(string filePath, ArExperienceGroup arExperienceGroup)
        {
            using (Stream stream = File.Open(filePath, FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, arExperienceGroup);
            }
        }

        public static ArExperienceGroup OpenFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (Stream stream = File.Open(filePath, FileMode.Open))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    return (ArExperienceGroup)binaryFormatter.Deserialize(stream);
                }
            }
            else
            {
                return null;
            }
        }

        private static Features ParseFeatures(JsonArray featuresArray)
        {
            Features features = 0;
            foreach (JsonValue feature in featuresArray)
            {
                switch ((string)feature)
                {
                    case "geo":
                        features |= Features.Geo;
                        break;
                    case "image_tracking":
                        features |= Features.ImageTracking;
                        break;
                    case "instant_tracking":
                        features |= Features.InstantTracking;
                        break;
                    case "object_tracking":
                        features |= Features.ObjectTracking;
                        break;
                }
            }
            return features;
        }

        private static CameraPosition ParseCameraPosition(JsonObject startupConfig)
        {
            CameraPosition cameraPosition = CameraPosition.Default;

            if (startupConfig.TryGetValue("camera_position", out JsonValue cameraPositionValue))
            {
                switch ((string)cameraPositionValue)
                {
                    case "front":
                        cameraPosition = CameraPosition.Front;
                        break;
                    case "back":
                        cameraPosition = CameraPosition.Back;
                        break;
                }
            }

            return cameraPosition;
        }

        private static CameraResolution ParseCameraResolution(JsonObject startupConfig)
        {
            CameraResolution cameraResolution = CameraResolution.SD_640x480;

            if (startupConfig.TryGetValue("camera_resolution", out JsonValue cameraResolutionValue))
            {
                switch ((string)cameraResolutionValue)
                {
                    case "sd_640x480":
                        cameraResolution = CameraResolution.SD_640x480;
                        break;
                    case "hd_1280x720":
                        cameraResolution = CameraResolution.HD_1280x720;
                        break;
                    case "full_hd_1920x1080":
                        cameraResolution = CameraResolution.Full_HD_1920x1080;
                        break;
                    case "auto":
                        cameraResolution = CameraResolution.Auto;
                        break;
                }
            }

            return cameraResolution;
        }
    }
}
