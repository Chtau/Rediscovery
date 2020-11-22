using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Feature.Desktop.MediaPlayer.Models
{
    public class ProfileConfiguration
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public List<CommandConfiguration.CommandTypes> CommandAvailable { get; set; }
        public string ProcessName { get; set; }
        public string ApplicationPath { get; set; }
        public CommandConfiguration CommandConfiguration { get; set; }
    }
}
