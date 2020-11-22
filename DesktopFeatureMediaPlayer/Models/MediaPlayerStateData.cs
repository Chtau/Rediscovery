using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Feature.Desktop.MediaPlayer.Models
{
    public class MediaPlayerStateData
    {
        public bool ProcessRunning { get; set; }

        public string Title { get; set; }
        public string Artist { get; set; }
        public string Info { get; set; }
    }
}
