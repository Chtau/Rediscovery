using System;
using System.Collections.Generic;
using System.Text;
using static SharedCoreModels.FeatureModels.MediaPlayer.CommandConfiguration;

namespace SharedCoreModels.FeatureModels.MediaPlayer
{
    public class ClientCommandSendModel
    {
        public Guid ProfileId { get; set; }

        public CommandTypes Command { get; set; }

        public ClientCommandSendModel()
        {

        }

        public ClientCommandSendModel(Guid profileId, CommandTypes command) : this()
        {
            ProfileId = profileId;
            Command = command;
        }
    }
}
