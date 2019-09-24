using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Rediscovery.Features.Authentication.Models
{
    public class AuthenticationKeyViewModel : BaseViewModel
    {
        private Features.Authentication.IConnect auth => DependencyService.Get<Features.Authentication.IConnect>() ?? new Features.Authentication.Connect();

        public Command KeyVerify { get; }

        public AuthenticationKey Item { get; set; }

        public AuthenticationKeyViewModel(Guid connectionId)
        {
            Item = new AuthenticationKey
            {
                ConnectionId = connectionId
            };
            KeyVerify = new Command(async () =>
            {
                await auth.ValidateKey(Item.ConnectionId, Item.Key);
            });
        }
    }
}
