using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels.FeatureModels.MediaPlayer
{
    public class ProfileConfiguration : Profile
    {
        public string ProcessName { get; set; }
        public string ApplicationPath { get; set; }
        public CommandConfiguration CommandConfiguration { get; set; }
    }
}
