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

        // TODO: Show quick connect menu (last connected or near know network or if we have only one configuration)
        // if we are connect show to which configuration we are connected
        // if we have no configuration inform the user that we must add a configuration
    }
}
