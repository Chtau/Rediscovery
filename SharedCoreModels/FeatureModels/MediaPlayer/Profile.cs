using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels.FeatureModels.MediaPlayer
{
    public class Profile
    {
        public Guid Id { get; set; }
        public string ProcessName { get; set; }
        public string ApplicationPath { get; set; }
        public CommandConfiguration CommandConfiguration { get; set; }
    }
}
