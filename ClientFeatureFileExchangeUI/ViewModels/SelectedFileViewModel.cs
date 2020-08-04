using System;
using System.Collections.Generic;
using System.Text;

namespace ClientFeatureFileExchangeUI.ViewModels
{
    public class SelectedFileViewModel : ViewModelBase
    {
        public event EventHandler<SelectedFileViewModel> DeleteFile;
        public event EventHandler<SelectedFileViewModel> OpenFile;

        public string FileName { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }

        public void DeleteSelectReceivedFile()
        {
            DeleteFile?.Invoke(this, this);
        }

        public void OpenSelectedReceivedFile()
        {
            OpenFile?.Invoke(this, this);
        }
    }
}
