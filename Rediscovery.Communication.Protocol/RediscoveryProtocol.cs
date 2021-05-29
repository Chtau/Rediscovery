using Rediscovery.Communication.Protocol.Internal;
using Rediscovery.Communication.Protocol.Internal.Listener;
using Rediscovery.Communication.Protocol.Internal.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Rediscovery.Communication.Protocol
{
    // TODO: https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example

    /* Bash
     * Listen via Netcat: nc -l -p 11000
     * Write via Netcat: echo 'test<EOF>' | sudo  netcat 192.168.1.102 11000
     */

    public class RediscoveryProtocol : IRediscoveryProtocol
    {
        private readonly IProtocolLogger _logger;
        private readonly DiscoveryListener _discoveryListener;
        private readonly IListener _dataListener;
        private readonly IListener _lowDataListener;
        private readonly DiscoverySender _discoverySender;
        private readonly ISender _dataSender;
        private readonly ISender _lowDataSender;
        private readonly ISerializer _serializer;
        private readonly IPackagePipeline _packagePipeline;

        private Models.Configuration configuration;
        

        public RediscoveryProtocol(IProtocolLogger protocolLogger = null, ISerializer serializer = null)
        {
            _logger = protocolLogger ?? new Internal.ProtocolLogger();
            _serializer = serializer ?? new Serializer(_logger);
            _packagePipeline = new PackagePipeline(_logger, _serializer);

            _discoveryListener = new DiscoveryListener(_logger, _packagePipeline);
            _dataListener = new DataListener(_logger, _packagePipeline);
            _lowDataListener = new LowDataListener(_logger, _packagePipeline);
            _discoverySender = new DiscoverySender(_logger, _packagePipeline);
            _dataSender = new DataSender(_logger, _packagePipeline);
            _lowDataSender = new LowDataSender(_logger, _packagePipeline);
        }

        public ConnectionState Connect(Connection connection)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return ConnectionState.Unkown;
        }

        public bool Disconnect()
        {
            throw new NotImplementedException();
        }

        public object GetConnectionInfo()
        {
            throw new NotImplementedException();
        }

        public object GetDiagnosticData()
        {
            throw new NotImplementedException();
        }

        public void NewDevices(Action<object> deviceCallback)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void Listen(Action<Transfer> receivedCallback)
        {
            try
            {
                _dataListener.StateCompleteListener((array) =>
                {
                    receivedCallback?.Invoke(new Transfer
                    {
                        Content = array
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void LowLatencyListen(Action<Transfer> receivedCallback)
        {
            try
            {
                _lowDataListener.StateCompleteListener((array) =>
                {
                    receivedCallback?.Invoke(new Transfer
                    {
                        Content = array
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public TransportState LowLatencySend(Transfer transfer)
        {
            throw new NotImplementedException();
        }

        public TransportState LowLatencyStream(Action<object> streamData)
        {
            throw new NotImplementedException();
        }

        public void Send(Transfer transfer, Action<TransportState> successCallback = null)
        {
            try
            {
                _dataSender.Send(transfer.Content, transfer.IP, configuration.Data.Connection.ListenPort, (success) =>
                {
                    successCallback?.Invoke(success);
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                successCallback?.Invoke(TransportState.Error);
            }
        }

        public void Start(Models.Configuration configuration)
        {
            try
            {
                this.configuration = configuration ?? new Models.Configuration();
                /*if (this.configuration.Discovery == null)
                    this.configuration.Discovery = new Models.DiscoveryConfiguration();
                if (this.configuration.Data == null)
                    this.configuration.Data = new Models.DataConfiguration();
                if (this.configuration.LowData == null)
                    this.configuration.LowData = new Models.LowDataConfiguration();*/
                _discoverySender.Initialize(this.configuration.Discovery);
                _dataSender.Initialize(this.configuration.Data);
                _lowDataSender.Initialize(this.configuration.LowData);
                _discoveryListener.Initialize(this.configuration.Discovery);
                _dataListener.Initialize(this.configuration.Data);
                _lowDataListener.Initialize(this.configuration.LowData);
                
                // start listen for portocol data and discovery requests
                OnListenDiscovery();
                OnListenData();
                //OnListenLowData();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public TransportState Stream(Action<object> streamData)
        {
            throw new NotImplementedException();
        }

        private void OnListenDiscovery()
        {
            try
            {
                _discoveryListener.Start();
                _discoverySender.Start();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnListenData()
        {
            try
            {
                _dataListener.Start();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnListenLowData()
        {
            try
            {
                _lowDataListener.Start();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
