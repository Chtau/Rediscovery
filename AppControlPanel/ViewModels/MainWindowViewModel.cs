using System;
using System.Collections.Generic;
using System.Text;

namespace AppControlPanel.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public System.Collections.ObjectModel.ObservableCollection<string> Apps { get; set; } = new System.Collections.ObjectModel.ObservableCollection<string>();
    }
}
