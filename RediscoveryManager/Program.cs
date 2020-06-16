using CommunicationAuthenticationConsumer;
using System;

namespace RediscoveryManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var uiHandler = new UIHandler();
            uiHandler.Start(args);
        }
    }
}
