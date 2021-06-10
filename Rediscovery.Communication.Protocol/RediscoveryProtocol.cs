using Rediscovery.Communication.Protocol.Internal;
using Rediscovery.Communication.Protocol.Internal.Data;
using Rediscovery.Communication.Protocol.Internal.Device;
using Rediscovery.Communication.Protocol.Internal.Discovery;
using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Rediscovery.Communication.Protocol
{
    public class RediscoveryProtocol : IRediscoveryProtocol, IDisposable
    {
        private readonly IProtocolLogger _logger;
        private readonly DiscoveryListener _discoveryListener;
        private readonly DiscoverySender _discoverySender;
        private readonly ISerializer _serializer;
        private readonly IPackagePipeline _packagePipeline;
        private readonly IDiscoveryPipeline _discoveryPipeline;
        private readonly IDeviceManager _deviceManager;
        private readonly ICommunication _communication;
        private readonly ICommunication _communicationLarge;

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
#if DISCOVER
            _logger.Trace("Diagnostic => Discover is active");
#endif

            _logger = protocolLogger ?? new Internal.ProtocolLogger();
            _serializer = serializer ?? new Serializer(_logger);
            _deviceManager = new DeviceManager(_logger);
            _deviceManager.DeviceChanged += (obj, args) =>
            {
                DevicesChanged?.Invoke(this, args);
            };
            _communication = new TCPCommunication(_logger, _deviceManager);
            _communicationLarge = new TCPCommunication(_logger, _deviceManager, true);
            _packagePipeline = new PackagePipeline(_logger, _serializer, _communication, _communicationLarge, _deviceManager);
            _discoveryPipeline = new DiscoveryPipeline(_logger, _serializer);
            _discoveryListener = new DiscoveryListener(_logger, _discoveryPipeline, _deviceManager);
            _discoverySender = new DiscoverySender(_logger, _discoveryPipeline, _deviceManager);
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

        public void Listen<T>(Action<Transfer<T>> receivedCallback)
        {
            try
            {
                _packagePipeline.Incoming<T>((instance, identifer) =>
                {
                    receivedCallback?.Invoke(new Transfer<T>(identifer, instance));
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void Send<T>(Transfer<T> transfer, Action<TransportState> successCallback = null)
        {
            try
            {
                var device = _deviceManager.GetGreeting(transfer.DeviceIdentifier);
                _packagePipeline.Outgoing(transfer.Content, device);
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

                OnStartDiscovery();
                OnStartCommunication();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnStartDiscovery()
        {
            try
            {
                _discoverySender.Initialize(this.configuration.Discovery, this.configuration.Data.Connection, this.configuration.Data.ConnectionLargeData);
                _discoveryListener.Initialize(this.configuration.Discovery);

                _discoveryListener.Start();
                _discoverySender.Start();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnStartCommunication()
        {
            try
            {
                _communication.Initialize(this.configuration.Data.Connection);
                _communication.Start();
                _communicationLarge.Initialize(this.configuration.Data.ConnectionLargeData);
                _communicationLarge.Start();
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
                _deviceManager.SetIdentifier(Identifer);
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
                _discoverySender.Stop();
                _communication.Stop();
                _communicationLarge.Stop();
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
