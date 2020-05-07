using PluginFeature.Models;
using SharedCoreModels.FeatureModels.MediaPlayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using static SharedCoreModels.FeatureModels.MediaPlayer.CommandConfiguration;

namespace Rediscovery.Features.DesktopFeatures.FeaturePage.MediaPlayer
{
    public class MediaPlayerFeatureViewModel : BaseFeatureViewModel
    {
        private bool processIsRunning = false;

        public ObservableCollection<DeviceFeatureProfil> Profiles { get; set; } = new ObservableCollection<DeviceFeatureProfil>();

        private DeviceFeatureProfil selectedProfile;

        public DeviceFeatureProfil SelectedProfile
        {
            get { return selectedProfile; }
            set
            {
                SetProperty(ref selectedProfile, value);
                OnProfileChanged();
            }
        }

        private string currentTitle;

        public string CurrentTitle
        {
            get { return currentTitle; }
            set { SetProperty(ref currentTitle, value); }
        }

        private List<CommandTypes> commands;

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

        public MediaPlayerFeatureViewModel(Features.Connection.Models.ConnectionManifestFeature connectionManifestFeature) : base(connectionManifestFeature)
        {
            base.ReceivedData += MediaPlayerFeatureViewModel_ReceivedData;
            OnSetCommand();
            if (connectionManifestFeature.Profiles?.Count > 0)
            {
                foreach (var item in connectionManifestFeature.Profiles)
                {
                    Profiles.Add(item);
                }
                SelectedProfile = connectionManifestFeature.Profiles.First();
                OnProfileChanged();
            } else
            {
                Title = connectionManifestFeature.FeatureDisplayName;
            }
        }

        private void OnSetCommand()
        {
            PlayCommand = new Command(execute: () => Send(CommandTypes.PlayPause),
                canExecute: () => Commands.Contains(CommandTypes.PlayPause) && processIsRunning);

            FullscreenExitCommand = new Command(execute: () => Send(CommandTypes.FullscreenExit),
                canExecute: () => Commands.Contains(CommandTypes.FullscreenExit) && processIsRunning);

            FullscreenCommand = new Command(execute: () => Send(CommandTypes.Fullscreen),
                canExecute: () => Commands.Contains(CommandTypes.Fullscreen) && processIsRunning);

            NextCommand = new Command(execute: () => Send(CommandTypes.Next),
                canExecute: () => Commands.Contains(CommandTypes.Next) && processIsRunning);

            PreviousCommand = new Command(execute: () => Send(CommandTypes.Previous),
                canExecute: () => Commands.Contains(CommandTypes.Previous) && processIsRunning);

            StopCommand = new Command(execute: () => Send(CommandTypes.Stop),
                canExecute: () => Commands.Contains(CommandTypes.Stop) && processIsRunning);

            MuteCommand = new Command(execute: () => Send(CommandTypes.Mute),
                canExecute: () => Commands.Contains(CommandTypes.Mute) && processIsRunning);

            VolumneUpCommand = new Command(execute: () => Send(CommandTypes.VolumneUp),
                canExecute: () => Commands.Contains(CommandTypes.VolumneUp) && processIsRunning);

            VolumneDownCommand = new Command(execute: () => Send(CommandTypes.VolumneDown),
                canExecute: () => Commands.Contains(CommandTypes.VolumneDown) && processIsRunning);

            SpeedSlowerCommand = new Command(execute: () => Send(CommandTypes.SpeedSlower),
                canExecute: () => Commands.Contains(CommandTypes.SpeedSlower) && processIsRunning);

            SpeedFasterCommand = new Command(execute: () => Send(CommandTypes.SpeedFaster),
                canExecute: () => Commands.Contains(CommandTypes.SpeedFaster) && processIsRunning);

            JumpForwardCommand = new Command(execute: () => Send(CommandTypes.JumpForward),
                canExecute: () => Commands.Contains(CommandTypes.JumpForward) && processIsRunning);

            JumpBackwardCommand = new Command(execute: () => Send(CommandTypes.JumpBackward),
                canExecute: () => Commands.Contains(CommandTypes.JumpBackward) && processIsRunning);
            OnChangeCanExecute();
        }

        private void OnChangeCanExecute()
        {
            (PlayCommand as Command)?.ChangeCanExecute();
            (FullscreenExitCommand as Command)?.ChangeCanExecute();
            (FullscreenCommand as Command)?.ChangeCanExecute();
            (NextCommand as Command)?.ChangeCanExecute();
            (PreviousCommand as Command)?.ChangeCanExecute();
            (StopCommand as Command)?.ChangeCanExecute();
            (MuteCommand as Command)?.ChangeCanExecute();
            (VolumneUpCommand as Command)?.ChangeCanExecute();
            (VolumneDownCommand as Command)?.ChangeCanExecute();
            (SpeedSlowerCommand as Command)?.ChangeCanExecute();
            (SpeedFasterCommand as Command)?.ChangeCanExecute();
            (JumpForwardCommand as Command)?.ChangeCanExecute();
            (JumpBackwardCommand as Command)?.ChangeCanExecute();
        }

        public void Send(CommandTypes cmd)
        {
            base.Send(SelectedProfile?.Id, new ClientCommandSendModel(_connectionManifestFeature.FeatureId, SelectedProfile.Id, cmd));
        }


        private void MediaPlayerFeatureViewModel_ReceivedData(object sender, object e)
        {
            MediaPlayerStateData stateData = Newtonsoft.Json.JsonConvert.DeserializeObject<MediaPlayerStateData>(e?.ToString());
            if (stateData != null)// && SelectedProfile != null && string.Equals(SelectedProfile.Id, stateData.ProfileId, StringComparison.OrdinalIgnoreCase))
            {
                if (processIsRunning != stateData.ProcessRunning)
                {
                    processIsRunning = stateData.ProcessRunning;
                    OnChangeCanExecute();
                }
                CurrentTitle = stateData.CurrentTitle;
            }
        }

        private void OnProfileChanged()
        {
            processIsRunning = false;
            CurrentTitle = "";
            Title = SelectedProfile.DisplayName;
            if (!string.IsNullOrWhiteSpace(SelectedProfile.ProfileData?.ToString()))
            {
                try
                {
                    Commands = System.Text.Json.JsonSerializer.Deserialize<List<CommandTypes>>(SelectedProfile.ProfileData?.ToString());
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            OnChangeCanExecute();
        }
    }
}
