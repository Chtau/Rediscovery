using System;
using System.Collections.Generic;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Services.Logger))]
namespace Rediscovery.Services
{
    public class Logger : ILogger
    {
        public void Error(Exception exception)
        {
            System.Diagnostics.Debug.Fail(exception.ToString());
        }

        public void Message(string message)
        {
            System.Diagnostics.Debug.Print(message);
        }
    }
}
