using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Rediscovery.Shared.Configurations
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
                if (value is System.Collections.IList list)
                {
                    var newVal = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                    var newJobj = Newtonsoft.Json.Linq.JArray.Parse(newVal);
                    Newtonsoft.Json.Linq.JToken jToken = jsonObj.SelectToken(sections[0]);
                    jToken.Replace(newJobj);
                } else
                {
                    var newVal = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                    var newJobj = Newtonsoft.Json.Linq.JObject.Parse(newVal);
                    Newtonsoft.Json.Linq.JToken jToken = jsonObj.SelectToken(sections[0]);
                    jToken.Replace(newJobj);
                }
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
