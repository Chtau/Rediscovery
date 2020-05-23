using DesktopFeatureMediaPlayer.Models;
using SharedFeatureFunctions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopFeatureMediaPlayer
{
    public static class MediaPlayerDefaultProfiles
    {
        public static List<ProfileConfiguration> GetProfileConfigurations(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                SaveProfileConfigurations(path, new List<ProfileConfiguration>
                {
                    VLC(),
                    Plex()
                });
            }
            var retVal = new List<ProfileConfiguration>();
            var loaded = OnLoadConfiguration(path);
            if (loaded?.Count > 0)
                retVal.AddRange(loaded);
            return retVal;
        }

        public static void SaveProfileConfigurations(string path, List<ProfileConfiguration> profiles)
        {
            var jsonProfiles = Newtonsoft.Json.JsonConvert.SerializeObject(profiles);
            System.IO.File.WriteAllText(path, jsonProfiles);
        }

        private static List<ProfileConfiguration> OnLoadConfiguration(string path)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProfileConfiguration>>(System.IO.File.ReadAllText(path));
        }

        private static ProfileConfiguration VLC()
        {
            var vlcId = new Guid("4D7A3004-F4F7-4B43-8DF1-2B9CA73F8991");
            var vlcCmdConfig = new CommandConfiguration
            {
                ProfileId = vlcId,
                CommandKeys = new Dictionary<CommandConfiguration.CommandTypes, KeyCode[]>()
            };
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.None, null);
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.Fullscreen, new KeyCode[] { KeyCode.KEY_F });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.PlayPause, new KeyCode[] { KeyCode.SPACE_BAR });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.FullscreenExit, new KeyCode[] { KeyCode.ESC });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.Next, new KeyCode[] { KeyCode.KEY_N });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.Previous, new KeyCode[] { KeyCode.KEY_P });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.Stop, new KeyCode[] { KeyCode.KEY_S });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.Mute, new KeyCode[] { KeyCode.KEY_M });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.VolumneUp, new KeyCode[] { KeyCode.CONTROL, KeyCode.UP });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.VolumneDown, new KeyCode[] { KeyCode.CONTROL, KeyCode.DOWN });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.SpeedSlower, new KeyCode[] { KeyCode.OEM_MINUS });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.SpeedFaster, new KeyCode[] { KeyCode.OEM_PLUS });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.JumpForward, new KeyCode[] { KeyCode.RIGHT });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.JumpBackward, new KeyCode[] { KeyCode.LEFT });
            var profile = new ProfileConfiguration
            {
                ApplicationPath = null,
                DisplayName = "VLC",
                Id = vlcId,
                ProcessName = "vlc",
                CommandConfiguration = vlcCmdConfig
            };
            profile.CommandAvailable = vlcCmdConfig.GetConfiguratedCommands();
            return profile;
        }

        private static ProfileConfiguration Plex()
        {
            var vlcId = new Guid("CC9033CA-02CE-48A6-8393-1C4838D9C1E8");
            var vlcCmdConfig = new CommandConfiguration
            {
                ProfileId = vlcId,
                CommandKeys = new Dictionary<CommandConfiguration.CommandTypes, KeyCode[]>()
            };
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.None, null);
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.Fullscreen, new KeyCode[] { KeyCode.SHIFT, KeyCode.F11 });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.PlayPause, new KeyCode[] { KeyCode.SPACE_BAR });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.FullscreenExit, new KeyCode[] { KeyCode.SHIFT, KeyCode.F11 });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.Next, new KeyCode[] { KeyCode.RIGHT, KeyCode.DECIMAL });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.Previous, new KeyCode[] { KeyCode.LEFT, KeyCode.OEM_COMMA });
            //vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.Stop, new KeyCode[] { KeyCode.KEY_S });
            //vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.Mute, new KeyCode[] { KeyCode.KEY_M });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.VolumneUp, new KeyCode[] { KeyCode.UP });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.VolumneDown, new KeyCode[] { KeyCode.DOWN });
            //vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.SpeedSlower, new KeyCode[] { KeyCode.OEM_MINUS });
            //vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.SpeedFaster, new KeyCode[] { KeyCode.OEM_PLUS });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.JumpForward, new KeyCode[] { KeyCode.RIGHT });
            vlcCmdConfig.CommandKeys.Add(CommandConfiguration.CommandTypes.JumpBackward, new KeyCode[] { KeyCode.LEFT });
            var profile = new ProfileConfiguration
            {
                ApplicationPath = null,
                DisplayName = "Plex",
                Id = vlcId,
                ProcessName = "PlexMediaPlayer",
                CommandConfiguration = vlcCmdConfig
            };
            profile.CommandAvailable = vlcCmdConfig.GetConfiguratedCommands();
            return profile;
        }
    }
}
