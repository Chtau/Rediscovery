using System;
using System.Collections.Generic;
using System.Text;
using static SharedCoreModels.FeatureModels.MediaPlayer.CommandConfiguration;

namespace SharedCoreModels.FeatureModels.MediaPlayer
{
    public class Profile
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public List<CommandTypes> CommandAvailable { get; set; }
    }
}
