using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using static SharedCoreModels.FeatureModels.MediaPlayer.CommandConfiguration;

namespace Rediscovery.Features.DesktopFeatures.FeaturePage.MediaPlayer
{
    public class MediaPlayerFeatureViewModel : BaseFeatureViewModel
    {
        private bool processIsRunning = false;

        List<CommandTypes> commands;
        public List<CommandTypes> Commands
        {
            get { return commands; }
            set { SetProperty(ref commands, value); }
        }

        public ICommand PlayCommand { get; set; }
        public ICommand FullscreenExitCommand { get; set; }
        public ICommand FullscreenCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public ICommand PreviousCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand MuteCommand { get; set; }
        public ICommand VolumneUpCommand { get; set; }
        public ICommand VolumneDownCommand { get; set; }
        public ICommand SpeedSlowerCommand { get; set; }
        public ICommand SpeedFasterCommand { get; set; }
        public ICommand JumpForwardCommand { get; set; }
        public ICommand JumpBackwardCommand { get; set; }

        public MediaPlayerFeatureViewModel(Authentication.Models.ConnectionManifestFeature connectionManifestFeature) : base(connectionManifestFeature)
        {
            base.ReceivedData += MediaPlayerFeatureViewModel_ReceivedData;
            Title = connectionManifestFeature.FeatureDisplayName;
            Commands = System.Text.Json.JsonSerializer.Deserialize<List<CommandTypes>>(connectionManifestFeature.SettingsObject?.ToString());
            OnSetCommand();
            Send(CommandTypes.None);
        }

        private void OnSetCommand()
        {
            PlayCommand = new Command(execute: () =>
            {
                Send(CommandTypes.PlayPause);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.PlayPause) && processIsRunning;
            });
            FullscreenExitCommand = new Command(execute: () =>
            {
                Send(CommandTypes.FullscreenExit);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.FullscreenExit) && processIsRunning;
            });
            FullscreenCommand = new Command(execute: () =>
            {
                Send(CommandTypes.Fullscreen);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.Fullscreen) && processIsRunning;
            });
            NextCommand = new Command(execute: () =>
            {
                Send(CommandTypes.Next);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.Next) && processIsRunning;
            });
            PreviousCommand = new Command(execute: () =>
            {
                Send(CommandTypes.Previous);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.Previous) && processIsRunning;
            });
            StopCommand = new Command(execute: () =>
            {
                Send(CommandTypes.Stop);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.Stop) && processIsRunning;
            });
            MuteCommand = new Command(execute: () =>
            {
                Send(CommandTypes.Mute);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.Mute) && processIsRunning;
            });
            VolumneUpCommand = new Command(execute: () =>
            {
                Send(CommandTypes.VolumneUp);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.VolumneUp) && processIsRunning;
            });
            VolumneDownCommand = new Command(execute: () =>
            {
                Send(CommandTypes.VolumneDown);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.VolumneDown) && processIsRunning;
            });
            SpeedSlowerCommand = new Command(execute: () =>
            {
                Send(CommandTypes.SpeedSlower);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.SpeedSlower) && processIsRunning;
            });
            SpeedFasterCommand = new Command(execute: () =>
            {
                Send(CommandTypes.SpeedFaster);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.SpeedFaster) && processIsRunning;
            });
            JumpForwardCommand = new Command(execute: () =>
            {
                Send(CommandTypes.JumpForward);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.JumpForward) && processIsRunning;
            });
            JumpBackwardCommand = new Command(execute: () =>
            {
                Send(CommandTypes.JumpBackward);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.JumpBackward) && processIsRunning;
            });
            OnChangeCanExecute();
        }

        private void OnChangeCanExecute()
        {
            (PlayCommand as Command).ChangeCanExecute();
            (FullscreenExitCommand as Command).ChangeCanExecute();
            (FullscreenCommand as Command).ChangeCanExecute();
            (NextCommand as Command).ChangeCanExecute();
            (PreviousCommand as Command).ChangeCanExecute();
            (StopCommand as Command).ChangeCanExecute();
            (MuteCommand as Command).ChangeCanExecute();
            (VolumneUpCommand as Command).ChangeCanExecute();
            (VolumneDownCommand as Command).ChangeCanExecute();
            (SpeedSlowerCommand as Command).ChangeCanExecute();
            (SpeedFasterCommand as Command).ChangeCanExecute();
            (JumpForwardCommand as Command).ChangeCanExecute();
            (JumpBackwardCommand as Command).ChangeCanExecute();
        }

        public void Send(CommandTypes cmd)
        {
            base.Send(new SharedCoreModels.FeatureModels.MediaPlayer.ClientCommandSendModel(_connectionManifestFeature.FeatureId, cmd));
        }


        private void MediaPlayerFeatureViewModel_ReceivedData(object sender, object e)
        {
            SharedCoreModels.FeatureModels.MediaPlayer.MediaPlayerStateData stateData = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCoreModels.FeatureModels.MediaPlayer.MediaPlayerStateData>(e?.ToString());
            if (stateData != null)
            {
                if (processIsRunning != stateData.ProcessRunning)
                {
                    processIsRunning = stateData.ProcessRunning;
                    OnChangeCanExecute();
                }
            }
        }
    }
}
