using System;
using System.Collections.Generic;
using System.Text;

namespace SharedConfigurations.DesktopService.Models
{
    public class AppConfiguration
    {
        public const string SectionName = "AppSettings";

        public string AppDataFolder { get; set; }

        public string ServiceDisplayName { get; set; }

        public string[] Plugins { get; set; }
    }
}
