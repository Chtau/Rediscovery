using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels.FeatureModels.MediaPlayer
{
    public class MediaPlayerStateData
    {
        public bool ProcessRunning { get; set; }

        public string CurrentTitle { get; set; }
    }
}
