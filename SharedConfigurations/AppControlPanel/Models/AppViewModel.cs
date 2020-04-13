using System;
using System.Collections.Generic;
using System.Text;

namespace SharedConfigurations.AppControlPanel.Models
{
    public class AppViewModel
    {
        public const string SectionName = "Apps";

        public string Name { get; set; }
        public string ExecuteableName { get; set; }
        public string SearchDirectory { get; set; }
        public string ExecuteArguments { get; set; }
        public string RunAs { get; set; }
    }
}
