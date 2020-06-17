using CommunicationAuthenticationConsumer;
using RediscoveryManager.Service;
using System;

namespace RediscoveryManager
{
    class Program
    {
        static void Main(string[] args)
        {
            IManager manager = new Manager(null);
            var uiHandler = new UIHandler(manager);
            uiHandler.Start(args);
        }
    }
}
