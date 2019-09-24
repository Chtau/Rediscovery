using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Rediscovery.Features.Authentication.Models
{
    public class AuthenticationKeyViewModel : BaseViewModel
    {
        public event EventHandler ShouldClose;

        private Features.Authentication.IConnect auth => DependencyService.Get<Features.Authentication.IConnect>() ?? new Features.Authentication.Connect();

        public Command KeyVerify { get; }

        public AuthenticationKey Item { get; set; }

        public AuthenticationKeyViewModel(Guid connectionId)
        {
            auth.HelloReceived += Auth_HelloReceived;
            Item = new AuthenticationKey
            {
                ConnectionId = connectionId,
                ConnectionState = SharedCoreModels.Enums.ConnectionState.WaitForApprovel,
                ShowState = true,
            };
            KeyVerify = new Command(async () =>
            {
                await auth.ValidateKey(Item.ConnectionId, Item.Key);
            });
        }

        private void Auth_HelloReceived(object sender, Connection e)
        {
            if (e.ConnectionState != SharedCoreModels.Enums.ConnectionState.OK)
            {
                Item.ShowState = true;
            }
            else
            {
                Item.ShowState = false;
                ShouldClose?.Invoke(this, EventArgs.Empty);
            }
            Item.ConnectionState = e.ConnectionState;
        }
    }
}
