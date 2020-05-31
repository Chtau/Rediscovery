using System;
using System.Threading.Tasks;

namespace TestSignalR
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var connectionConfiguration = new CommunicationBase.ConnectionConfiguration
            {
                Address = "192.168.1.100:44342",
                DisplayName = "hub",
                Id = Guid.NewGuid(),
                State = CommunicationBase.ConnectionState.None,
                Token = null
            };
            var _hub = new CommunicationResourceConsumer.Hub();
            _hub.Init(SharedBase.Logging.DiagnosticsLoggerProvider.Instance, "/remote/resource/hub", CommunicationBase.Protocol.HTTPS);
            _hub.ConnectionStateChanged += _hub_ConnectionStateChanged;

            InitServiceConnection(_hub, connectionConfiguration);
            Console.ReadKey();
        }

        private static void _hub_ConnectionStateChanged(object sender, bool e)
        {
            Console.WriteLine("SignalR connection changed:" + e);
        }

        private static async Task<bool> InitServiceConnection(CommunicationResourceConsumer.IHub _hub, CommunicationBase.ConnectionConfiguration connectionConfiguration)
        {
            try
            {
                await _hub.Disconnect();
                _hub.Authenticate(connectionConfiguration.DisplayName, connectionConfiguration, (resultModel, state) =>
                {
                    if (state)
                    {
                        connectionConfiguration.Token = resultModel.Token;
                        _hub.Connect(connectionConfiguration.DisplayName, connectionConfiguration, (listener) =>
                        {
                            if (listener)
                            {
                                _hub.RequestAllData();
                            }
                            else
                            {
                                string msg = "Listener response not valid";
                            }
                        });
                    }
                    else
                    {
                        string msg = "Could not Authenticate for remote resource access";
                    }
                });
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
                return false;
            }
        }
    }
}
