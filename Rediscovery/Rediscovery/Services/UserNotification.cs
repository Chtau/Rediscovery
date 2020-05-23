using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Services.UserNotification))]
namespace Rediscovery.Services
{
    public class UserNotification : BaseService, IUserNotification
    {
        public void ShowToast(string message)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    Plugin.Toast.CrossToastPopUp.Current.ShowToastMessage(message);
                });
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
