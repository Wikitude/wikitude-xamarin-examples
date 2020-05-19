using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace XamarinExampleApp
{
    [Serializable]
    public class ArExperience
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public Features FeaturesMask { get; private set; }
        public CameraPosition CameraPosition { get; private set; }
        public CameraResolution CameraResolution { get; private set; }
        public CameraFocusMode CameraFocusMode { get; private set; }
        public string Extension { get; private set; }
        public bool Camera2Enabled { get; private set; }

        public ArExperience(string name, string path, Features features, CameraPosition position = CameraPosition.Default, CameraResolution resolution = CameraResolution.SD_640x480,
                            CameraFocusMode focusMode = CameraFocusMode.AutofocusContinuous, string extension = null, bool camera2Enabled = true)
        {
            Name = name;
            Path = path;
            FeaturesMask = features;
            CameraPosition = position;
            CameraResolution = resolution;
            CameraFocusMode = focusMode;
            Extension = extension;
            Camera2Enabled = camera2Enabled;
        }

        public static byte[] Serialize(ArExperience experience) {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, experience);
                return stream.ToArray();
            }
        }

        public static ArExperience Deserialize(byte[] data)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);
                var experience = formatter.Deserialize(stream) as ArExperience;
                return experience;
            }
        }
    }

    [Flags, Serializable]
    public enum Features 
    {
        Geo = 1,
        ImageTracking = 2,
        InstantTracking = 4,
        ObjectTracking = 8
    }

    [Serializable]
    public enum CameraPosition 
    {
        Default,
        Front,
        Back
    }

    [Serializable]
    public enum CameraResolution
    {
        SD_640x480,
        HD_1280x720,
        Full_HD_1920x1080,
        Auto
    }

    [Serializable]
    public enum CameraFocusMode
    {
        AutofocusOnce,
        AutofocusContinuous,
        AutofocusOff
    }
}
