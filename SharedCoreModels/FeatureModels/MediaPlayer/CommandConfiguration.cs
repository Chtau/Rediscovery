using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels.FeatureModels.MediaPlayer
{
    public class CommandConfiguration
    {
        public enum CommandTypes
        {
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
        public Dictionary<CommandTypes, KeyCodes.KeyCode> CommandKeys { get; set; }
    }
}
