using System;
using System.Collections.Generic;
using System.Text;

namespace ClientFeatureFileExchange
{
    public class Configuration
    {
        public string WorkingFolder { get; set; }
        public string StartProcessName { get; set; }
        public string FallbackFileExtensionContent { get; set; }
        public string FallbackFileExtensionHtml { get; set; }
        public string FallbackFileExtensionText { get; set; }

        public static Configuration GetConfigurations(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                SaveConfigurations(path, new Configuration
                {
                    WorkingFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    StartProcessName = "explorer.exe",
                    FallbackFileExtensionContent = "tmp",
                    FallbackFileExtensionHtml = "html",
                    FallbackFileExtensionText = "txt"
                });
            }
            return OnLoadConfiguration(path);
        }

        private static void SaveConfigurations(string path, Configuration profiles)
        {
            var jsonProfiles = Newtonsoft.Json.JsonConvert.SerializeObject(profiles);
            System.IO.File.WriteAllText(path, jsonProfiles);
        }

        private static Configuration OnLoadConfiguration(string path)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Configuration>(System.IO.File.ReadAllText(path));
        }
    }
}
