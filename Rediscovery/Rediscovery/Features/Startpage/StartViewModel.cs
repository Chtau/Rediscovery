using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Rediscovery.Features.Startpage
{
    public class StartViewModel : BaseViewModel
    {
        public Command OpenUrlCommand { get; set; }

        public StartViewModel()
        {
            OpenUrlCommand = new Command<string>(async (url) =>
            {
                await Launcher.OpenAsync(url);
            });
        }
    }
}
