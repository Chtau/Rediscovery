using Avalonia.Controls;
using Avalonia.Dialogs;
using Avalonia.Threading;
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
        private readonly IPCPipe.IPipeExchange _pipeExchange;

        public ObservableCollection<SelectedFileViewModel> SendFiles { get; } = new ObservableCollection<SelectedFileViewModel>();
        public ObservableCollection<SelectedFileViewModel> ReceivedFiles { get; } = new ObservableCollection<SelectedFileViewModel>();
        
        public MainWindowViewModel(Window window)
        {
            _window = window;

            _pipeExchange = new IPCPipe.PipeExchange();
            _pipeExchange.Init("7C7BE7CA-DE13-4975-A099-C64FA1581E4A", "in", "out");
            _pipeExchange.DataReceived += (obj, args) =>
            {
                //System.Diagnostics.Debug.Print($"IPCServer on {nameof(_window)} Hub received data:{args}");
                OnHandleInputData(args);
            };
            //_pipeExchange.Send($"Send from Client {DateTime.Now}");
        }

        private void OnHandleInputData(string pipeData)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(pipeData) && pipeData.Contains(";"))
                {
                    string[] comData = pipeData.Split(";");
                    string command = comData[0].ToLower();
                    string content = comData[1];

                    switch (command)
                    {
                        case "file":
                            if (System.IO.File.Exists(content))
                            {
                                string fileName = System.IO.Path.GetFileName(content);
                                string path = System.IO.Path.GetDirectoryName(content);
                                var model = new SelectedFileViewModel
                                {
                                    FileName = fileName,
                                    Path = path,
                                    FullPath = content
                                };
                                model.OpenFile += Model_OpenFile;
                                model.DeleteFile += Model_DeleteFile;
                                Dispatcher.UIThread.InvokeAsync(() =>
                                {
                                    ReceivedFiles.Add(model);
                                });
                            } else
                            {
                                System.Diagnostics.Debug.Print($"Received File command but file does not exist (File:{content}).");
                            }
                            break;
                        case "text":
                            break;
                        case "url":
                            break;
                        default:
                            System.Diagnostics.Debug.Print($"Unknown command received Command:{command}");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
            }
        }

        private void Model_DeleteFile(object sender, SelectedFileViewModel e)
        {
            try
            {
                _pipeExchange.Send($"delete;{e.FullPath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
            }
        }

        private void Model_OpenFile(object sender, SelectedFileViewModel e)
        {
            try
            {
                _pipeExchange.Send($"open;{e.FullPath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
            }
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

        public void ReceivedCheckChanges()
        {
            try
            {
                // TODO: implement received files
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
            }
        }
    }
}
