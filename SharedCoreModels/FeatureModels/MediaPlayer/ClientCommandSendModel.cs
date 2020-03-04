using System;
using System.Collections.Generic;
using System.Text;
using static SharedCoreModels.FeatureModels.MediaPlayer.CommandConfiguration;

namespace SharedCoreModels.FeatureModels.MediaPlayer
{
    public class ClientCommandSendModel
    {
        public Guid FeatureId { get; set; }

        public string ProfileId { get; set; }

        public CommandTypes Command { get; set; }

        public ClientCommandSendModel()
        {

        }

        public ClientCommandSendModel(Guid featureId, string profileId, CommandTypes command) : this()
        {
            FeatureId = featureId;
            ProfileId = profileId;
            Command = command;
        }
    }
}
