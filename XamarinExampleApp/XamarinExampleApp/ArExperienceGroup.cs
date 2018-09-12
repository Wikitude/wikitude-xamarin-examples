using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace XamarinExampleApp
{
    [Serializable()]
    public class ArExperienceGroup
    {
        public List<ArExperience> ArExperiences { get; private set; }
        public string Name { get; private set; }

        public ArExperienceGroup(string groupName, List<ArExperience> arExperiences)
        {
            Name = groupName;
            ArExperiences = arExperiences;
        }

        public static byte[] Serialize(ArExperienceGroup experienceGroup)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, experienceGroup);
                return stream.ToArray();
            }
        }

        public static ArExperienceGroup Deserialize(byte[] data)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);
                var experienceGroup = formatter.Deserialize(stream) as ArExperienceGroup;
                return experienceGroup;
            }
        }
    }
}
