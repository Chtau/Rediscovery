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
        List<CommandTypes> commands;
        public List<CommandTypes> Commands
        {
            get { return commands; }
            set { SetProperty(ref commands, value); }
        }

        public ICommand PlayCommand;
        public ICommand FullscreenExitCommand;
        public ICommand FullscreenCommand;
        public ICommand NextCommand;
        public ICommand PreviousCommand;
        public ICommand StopCommand;
        public ICommand MuteCommand;
        public ICommand VolumneUpCommand;
        public ICommand VolumneDownCommand;
        public ICommand SpeedSlowerCommand;
        public ICommand SpeedFasterCommand;
        public ICommand JumpForwardCommand;
        public ICommand JumpBackwardCommand;

        public MediaPlayerFeatureViewModel(Authentication.Models.ConnectionManifestFeature connectionManifestFeature) : base(connectionManifestFeature)
        {
            Commands = connectionManifestFeature.SettingsObject as List<CommandTypes>;
            OnSetCommand();
        }

        private void OnSetCommand()
        {
            PlayCommand = new Command(execute: () =>
            {
                Send(CommandTypes.PlayPause);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.PlayPause);
            });
            FullscreenExitCommand = new Command(execute: () =>
            {
                Send(CommandTypes.FullscreenExit);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.FullscreenExit);
            });
            FullscreenCommand = new Command(execute: () =>
            {
                Send(CommandTypes.Fullscreen);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.Fullscreen);
            });
            NextCommand = new Command(execute: () =>
            {
                Send(CommandTypes.Next);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.Next);
            });
            PreviousCommand = new Command(execute: () =>
            {
                Send(CommandTypes.Previous);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.Previous);
            });
            StopCommand = new Command(execute: () =>
            {
                Send(CommandTypes.Stop);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.Stop);
            });
            MuteCommand = new Command(execute: () =>
            {
                Send(CommandTypes.Mute);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.Mute);
            });
            VolumneUpCommand = new Command(execute: () =>
            {
                Send(CommandTypes.VolumneUp);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.VolumneUp);
            });
            VolumneDownCommand = new Command(execute: () =>
            {
                Send(CommandTypes.VolumneDown);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.VolumneDown);
            });
            SpeedSlowerCommand = new Command(execute: () =>
            {
                Send(CommandTypes.SpeedSlower);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.SpeedSlower);
            });
            SpeedFasterCommand = new Command(execute: () =>
            {
                Send(CommandTypes.SpeedFaster);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.SpeedFaster);
            });
            JumpForwardCommand = new Command(execute: () =>
            {
                Send(CommandTypes.JumpForward);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.JumpForward);
            });
            JumpBackwardCommand = new Command(execute: () =>
            {
                Send(CommandTypes.JumpBackward);
            },
            canExecute: () =>
            {
                return Commands.Contains(CommandTypes.JumpBackward);
            });
        }

        public void Send(CommandTypes cmd)
        {
            base.Send(new SharedCoreModels.FeatureModels.MediaPlayer.ClientCommandSendModel(_connectionManifestFeature.FeatureId, cmd));
        }
    }
}
