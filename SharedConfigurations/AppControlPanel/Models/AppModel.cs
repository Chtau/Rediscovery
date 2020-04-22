using System;
using System.Collections.Generic;
using System.Text;

namespace SharedConfigurations.AppControlPanel.Models
{
    public class AppModel
    {
        public const string SectionName = "Apps";

        public string Name { get; set; }
        public string ExecuteableName { get; set; }
        public string SearchDirectory { get; set; }
        public string ExecuteArguments { get; set; }
        public string WorkingDirectory { get; set; }
        public string RunAs { get; set; }
        public bool? HideShell { get; set; }
        public bool? AutoStartWithPanel { get; set; }
        public bool? AutoStartWhenNotRunning { get; set; }
        public bool? UseCommandLine { get; set; }
        public string CommandLineCommand { get; set; }
        public string CommandLineWorkingDirectory { get; set; }
        public string ProcessName { get; set; }
    }
}
