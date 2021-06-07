using Rediscovery.Communication.Protocol.Internal;
using Rediscovery.Communication.Protocol.Internal.Listener;
using Rediscovery.Communication.Protocol.Internal.Sender;
using Rediscovery.Communication.Protocol.Models;
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

    public class RediscoveryProtocol : IRediscoveryProtocol, IDisposable
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
        private readonly IDiscoveryPipeline _discoveryPipeline;
        private readonly IDeviceManager _deviceManager;
        private readonly ICommunication _communication;

        private Models.Configuration configuration;
        private string identifer;
        private bool disposedValue;

        public event EventHandler<string> DevicesChanged;

        public List<DeviceGreeting> Devices => _deviceManager.Devices;
        public string Identifer 
        { 
            get
            {
                if (string.IsNullOrWhiteSpace(identifer))
                {
                    identifer = NewIdentifier();
                    OnChangedIdentifier();
                }
                return identifer;
            }
        }

        public RediscoveryProtocol(string identifer = null, IProtocolLogger protocolLogger = null, ISerializer serializer = null)
        {
            _logger = protocolLogger ?? new Internal.ProtocolLogger();
            _serializer = serializer ?? new Serializer(_logger);
            _deviceManager = new DeviceManager(_logger);
            _deviceManager.DeviceChanged += (obj, args) =>
            {
                DevicesChanged?.Invoke(this, args);
            };
            _communication = new TCPCommunication(_logger, _deviceManager);
            _packagePipeline = new PackagePipeline(_logger, _serializer, _communication);
            _discoveryPipeline = new DiscoveryPipeline(_logger, _serializer);
            _discoveryListener = new DiscoveryListener(_logger, _discoveryPipeline, _deviceManager);
            _dataListener = new DataListener(_logger, _packagePipeline);
            _lowDataListener = new LowDataListener(_logger, _packagePipeline);
            _discoverySender = new DiscoverySender(_logger, _discoveryPipeline);
            _dataSender = new DataSender(_logger, _packagePipeline);
            _lowDataSender = new LowDataSender(_logger, _packagePipeline);
            if (!string.IsNullOrWhiteSpace(identifer))
            {
                this.identifer = identifer;
                OnChangedIdentifier();
            }
            if (string.IsNullOrWhiteSpace(Identifer))
                throw new ArgumentNullException(nameof(Identifer), "Cloud not create a new Identifier");
        }

        public string NewIdentifier() => $"{Guid.NewGuid()}.{DateTime.Now}.{Environment.MachineName}".GetHashString().GetHashString(HashExtensions.HashAlgorithmTypes.MD5).Substring(0,16);

        public void Stop()
        {
            OnStop();
        }

        public object GetConnectionInfo()
        {
            throw new NotImplementedException();
        }

        public object GetDiagnosticData()
        {
            throw new NotImplementedException();
        }

        public void Listen<T>(Action<Transfer<T>> receivedCallback)
        {
            try
            {
                _dataListener.StateCompleteListener((result) =>
                {
                    //_discoveryListener.Devices.FirstOrDefault()
                    receivedCallback?.Invoke(new Transfer<T>(result.IP, _packagePipeline.Incoming<T>(result.Raw)));
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void LowLatencyListen<T>(Action<Transfer<T>> receivedCallback)
        {
            try
            {
                return;
                /*_lowDataListener.StateCompleteListener((array) =>
                {
                    receivedCallback?.Invoke(new Transfer<T>
                    {
                        Content = array
                    });
                });*/
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public TransportState LowLatencySend<T>(Transfer<T> transfer)
        {
            throw new NotImplementedException();
        }

        public void Send<T>(Transfer<T> transfer, Action<TransportState> successCallback = null)
        {
            try
            {
                var device = _deviceManager.GetGreeting(transfer.DeviceIdentifier);
                _dataSender.Send(transfer.Content, device, (success) =>
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
                
                _discoverySender.Initialize(this.configuration.Discovery, this.configuration.Data.Connection, this.configuration.LowData.Connection);
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

        public void SetMetadata(string identifer, string friendlyName, DeviceMetadata.IdiomType idiomType)
        {
            try
            {
                this.identifer = identifer;
                OnChangedIdentifier();
                _discoverySender.SetFriendlyName(friendlyName);
                _discoverySender.SetIdiom(idiomType);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnChangedIdentifier()
        {
            try
            {
                _discoverySender.SetIdentifier(Identifer);
                _discoveryListener.SetIdentifier(Identifer);
                _packagePipeline.SetIdentifier(Identifer);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnStop()
        {
            try
            {
                _discoveryListener.Stop();
                _dataListener.Stop();
                _lowDataListener.Stop();
                _discoverySender.Stop();
                _dataSender.Stop();
                _lowDataSender.Stop();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnStop();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
