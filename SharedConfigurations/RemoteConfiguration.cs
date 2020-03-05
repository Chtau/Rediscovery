using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharedConfigurations
{
    public static class RemoteConfiguration
    {
        public static void UpdateRemoteConfiguration<T>(string filePath, string key, T value)
        {
            string json = File.ReadAllText(filePath);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            List<string> sections = new List<string>();
            if (key.Contains(":"))
            {
                sections.AddRange(key.Split(':'));
            }
            else
            {
                sections.Add(key);
            }
            if (sections.Count > 0)
            {
                dynamic obj = jsonObj[sections[0]];
                for (int i = 1; i < sections.Count; i++)
                {
                    obj = obj[sections[i]];
                }

                var section = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(obj.ToString());
                if (section != null)
                    section = value;
            }
            else
            {
                //jsonObj[sectionPath] = value; // if no sectionpath just set the value
            }
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, output);
        }

        public static T ReadRemoteConfiguration<T>(string filePath, string key)
        {
            string json = File.ReadAllText(filePath);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            List<string> sections = new List<string>();
            if (key.Contains(":"))
            {
                sections.AddRange(key.Split(':'));
            }
            else
            {
                sections.Add(key);
            }
            if (sections.Count > 0)
            {
                dynamic obj = jsonObj[sections[0]];
                for (int i = 1; i < sections.Count; i++)
                {
                    obj = obj[sections[i]];
                }

                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(obj.ToString());
            }
            else
            {
                //jsonObj[sectionPath] = value; // if no sectionpath just set the value
                return default;
            }
        }
    }
}
