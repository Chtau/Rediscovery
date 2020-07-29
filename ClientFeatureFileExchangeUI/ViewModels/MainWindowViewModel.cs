using Avalonia.Controls;
using Avalonia.Dialogs;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ClientFeatureFileExchangeUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Window _window;

        public ObservableCollection<SelectedFileViewModel> SendFiles { get; } = new ObservableCollection<SelectedFileViewModel>();

        public MainWindowViewModel(Window window)
        {
            _window = window;
        }

        public async void OpenFileSelect()
        {
            try
            {
                var res = await new OpenFileDialog()
                {
                    Title = "Select Files / Folders",
                    AllowMultiple = true,
                }.ShowManagedAsync(_window, new ManagedFileDialogOptions
                {
                    AllowDirectorySelection = true
                });
                if (res?.Length > 0)
                {
                    foreach (var item in res)
                    {
                        string fileName = System.IO.Path.GetFileName(item);
                        string path = System.IO.Path.GetDirectoryName(item);
                        SendFiles.Add(new SelectedFileViewModel
                        {
                            FileName = fileName,
                            Path = path
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
            }
        }

        public void ClearFileFolderSelections()
        {
            try
            {
                SendFiles.Clear();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
            }
        }

        public void SendToDevice()
        {
            try
            {
                // TODO: implement send files/folders to the device via the plugin
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
            }
        }
    }
}
