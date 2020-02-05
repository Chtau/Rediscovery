using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels.FeatureModels.MediaPlayer
{
    public class CommandConfiguration
    {
        public enum CommandTypes
        {
            None,
            PlayPause,
            FullscreenExit,
            Fullscreen,
            Next,
            Previous,
            Stop,
            Mute,
            VolumneUp,
            VolumneDown,
            SpeedSlower,
            SpeedFaster,
            JumpForward,
            JumpBackward
        }

        public Guid ProfileId { get; set; }
        public Dictionary<CommandTypes, KeyCodes.KeyCode[]> CommandKeys { get; set; }

        public List<CommandTypes> GetConfiguratedCommands()
        {
            var retVal = new List<CommandTypes>();
            if (CommandKeys != null && CommandKeys.Count > 0)
            {
                foreach (var item in CommandKeys)
                {
                    if (item.Value != null && item.Value.Length > 0)
                        retVal.Add(item.Key);
                }
            }
            return retVal;
        }
    }
}
