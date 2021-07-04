using Rediscovery.Communication.Protocol.Internal.Encryption;
using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Protocol.Internal.Device
{
    internal class DeviceManager : IDeviceManager
    {
        private readonly IProtocolLogger _logger;
        private readonly IEncryption _encryption;
        private readonly ISerializer _serializer;
        private readonly List<DeviceGreetingReceived> _devices = new List<DeviceGreetingReceived>();
        private readonly TimeSpan deviceTimeoutOffset = TimeSpan.FromSeconds(10);
        private readonly TimeSpan waitBeforeTimeoutCheck = TimeSpan.FromSeconds(30);
        private readonly Dictionary<string, string> _deviceSymmetric = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _devicePublicKeys = new Dictionary<string, string>();
        private readonly Dictionary<string, AsymmetricDiffieHellman> _deviceDHInstances = new Dictionary<string, AsymmetricDiffieHellman>();

        private bool isTimeoutCheckRunning = false;
        private string currentIdentifer;

        public List<DeviceGreeting> Devices => _devices.Select(x => x.Device)?.ToList();

        public event EventHandler<string> DeviceChanged;
        public event EventHandler<string> DeviceIncomingPing;

        public DeviceManager(IProtocolLogger logger,
            IEncryption encryption,
            ISerializer serializer)
        {
            _logger = logger;
            _encryption = encryption;
            _serializer = serializer;
        }

        public DeviceGreetingReceived GetGreeting(string identifier)
        {
            try
            {
                identifier = identifier.ExactLength(16);
                return _devices.FirstOrDefault(x => string.Equals(x.Device.Identifier, identifier, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }

        public bool Change(DeviceGreeting deviceGreeting, IPEndPoint ipEndPoint)
        {
            try
            {
                if (deviceGreeting.Identifier.Length > 16)
                    deviceGreeting.Identifier = deviceGreeting.Identifier.ExactLength(16);
                if (string.Equals(currentIdentifer, deviceGreeting.Identifier))
                    return false;
                lock (_devices)
                {
                    var d = _devices.FirstOrDefault(x => x.Device.Identifier == deviceGreeting.Identifier);
                    if (d != null)
                    {
                        if (d.Update(deviceGreeting, ipEndPoint.Address.ToString()))
                        {
                            OnHandleTimeoutDevices();
                            DeviceChanged?.Invoke(this, deviceGreeting.Identifier);
                            return true;
                        }
                    }
                    else
                    {
                        _devices.Add(new DeviceGreetingReceived(deviceGreeting, ipEndPoint.Address.ToString()));
                        OnHandleTimeoutDevices();
                        DeviceChanged?.Invoke(this, deviceGreeting.Identifier);
                        return true;
                    }
                    DeviceIncomingPing?.Invoke(this, deviceGreeting.Identifier);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            OnHandleTimeoutDevices();
            return false;
        }

        private void OnHandleTimeoutDevices()
        {
            if (isTimeoutCheckRunning)
                return;
            isTimeoutCheckRunning = true;
            try
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(waitBeforeTimeoutCheck);
                        _devices.RemoveAll(x => x.Received < (DateTime.UtcNow - deviceTimeoutOffset));
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                    finally
                    {
                        isTimeoutCheckRunning = false;
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                isTimeoutCheckRunning = false;
            }
        }

        public void SetIdentifier(string identifier) => currentIdentifer = identifier.ExactLength(16);

        private string OnDeviceSymmetricPassword(string identifier)
        {
            try
            {
                identifier = identifier.ExactLength(16);
                if (_deviceSymmetric.ContainsKey(identifier))
                    return _deviceSymmetric[identifier];
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }

        /*public void AddOrUpdateDeviceSymmetric(string identifier, string password)
        {
            try
            {
                if (_deviceSymmetric.ContainsKey(identifier))
                    _deviceSymmetric[identifier] = password;
                else
                    _deviceSymmetric.Add(identifier, password);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }*/

        public string GetIP(string identifier)
        {
            try
            {
                identifier = identifier.ExactLength(16);
                return _devices.FirstOrDefault(x => string.Equals(x.Device.Identifier, identifier, StringComparison.OrdinalIgnoreCase))?.IP;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }

        /*public void AddOrUpdateDevicePublicKey(string identifier, string publicKey)
        {
            try
            {
                if (_devicePublicKeys.ContainsKey(identifier))
                    _devicePublicKeys[identifier] = publicKey;
                else
                    _devicePublicKeys.Add(identifier, publicKey);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }*/

        /*public string DevicePublicKey(string identifier)
        {
            try
            {
                if (_devicePublicKeys.ContainsKey(identifier))
                    return _devicePublicKeys[identifier];
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }*/

        public bool HandshakeRequired(string identifier)
        {
            try
            {
                identifier = identifier.ExactLength(16);
                return _deviceSymmetric.ContainsKey(identifier);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return false;
        }

        public byte[] Decrypt(byte[] cypher, string identifier)
        {
            try
            {
                identifier = identifier.ExactLength(16);
                var pw = OnDeviceSymmetricPassword(identifier);
                if (!string.IsNullOrWhiteSpace(pw))
                    return _encryption.DecryptSymmetric(pw, cypher);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }

        public byte[] Encrypt(byte[] raw, string identifier)
        {
            try
            {
                identifier = identifier.ExactLength(16);
                var pw = OnDeviceSymmetricPassword(identifier);
                if (!string.IsNullOrWhiteSpace(pw))
                    return _encryption.EncryptSymmetric(pw, raw);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }

        /*public string GetOrCreateSymmetricPassword(string identifier, bool addForLocalIdentifier = true)
        {
            try
            {
                var key = OnGetLocalRemoteIdentifier(identifier);
                var pw = OnDeviceSymmetricPassword(key);
                if (!string.IsNullOrWhiteSpace(pw))
                    return pw;
                var newPW = _encryption.CreatePassword();
                AddOrUpdateDeviceSymmetric(key, newPW);
                return newPW;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }*/

        private string OnGetLocalRemoteIdentifier(string identifier) => currentIdentifer + "@" + identifier.ExactLength(16);

        public void DHKeyReceived(byte[] publicKey, string identifier)
        {
            try
            {
                identifier = identifier.ExactLength(16);
                var coords = _serializer.Deserialize<byte[][]>(publicKey);
                AsymmetricDiffieHellman instance = null;
                if (!_deviceDHInstances.ContainsKey(identifier))
                {
                    _deviceDHInstances.Add(identifier, new AsymmetricDiffieHellman());
                    instance = _deviceDHInstances[identifier];
                    instance.CreateKeyPair();
                } else
                    instance = _deviceDHInstances[identifier];
                instance.SetPublicKey(new AsymmetricDiffieHellman.KeyCoords(coords[0], coords[1]));
                var sharedBytes = instance.GetSharedSecret();

                string password = Convert.ToBase64String(sharedBytes);
                if (!string.IsNullOrWhiteSpace(password))
                {
                    if (_deviceSymmetric.ContainsKey(identifier))
                        _deviceSymmetric[identifier] = password;
                    else
                        _deviceSymmetric.Add(identifier, password);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public byte[] GetDHPublicKey(string identifier)
        {
            try
            {
                identifier = identifier.ExactLength(16);
                AsymmetricDiffieHellman instance = null;
                if (!_deviceDHInstances.ContainsKey(identifier))
                {
                    instance = new AsymmetricDiffieHellman();
                    _deviceDHInstances.Add(identifier, instance);
                    instance.CreateKeyPair();
                } else
                {
                    instance = _deviceDHInstances[identifier];
                }
                var key = instance.LocalPublicKey;
                var keyValues = key.Get();
                return _serializer.Serialize(keyValues);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }
    }
}
