using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels.FeatureModels.VLC
{
    public class VLCCommandModel
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

        public CommandTypes Command { get; set; }
    }
}
