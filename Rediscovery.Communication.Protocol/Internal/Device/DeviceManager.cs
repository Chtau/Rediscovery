using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Protocol.Internal.Device
{
    internal class DeviceManager : IDeviceManager
    {
        private readonly IProtocolLogger _logger;
        private readonly List<DeviceGreetingReceived> _devices = new List<DeviceGreetingReceived>();
        private readonly TimeSpan deviceTimeoutOffset = TimeSpan.FromSeconds(10);
        private readonly TimeSpan waitBeforeTimeoutCheck = TimeSpan.FromSeconds(30);
        private readonly Dictionary<string, string> _deviceSymmetric = new Dictionary<string, string>();

        private bool isTimeoutCheckRunning = false;
        private string currentIdentifer;

        public List<DeviceGreeting> Devices => _devices.Select(x => x.Device)?.ToList();

        public event EventHandler<string> DeviceChanged;

        public DeviceManager(IProtocolLogger logger)
        {
            _logger = logger;
        }

        public DeviceGreetingReceived GetGreeting(string identifier)
        {
            try
            {
                return _devices.FirstOrDefault(x => string.Equals(x.Device.Identifier, identifier, StringComparison.OrdinalIgnoreCase));
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }

        public bool Change(DeviceGreeting deviceGreeting, IPEndPoint ipEndPoint)
        {
            try
            {
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
                    } finally
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

        public void SetIdentifier(string identifer) => currentIdentifer = identifer;

        public string DeviceSymmetricPassword(string identifer)
        {
            try
            {
                if (_deviceSymmetric.ContainsKey(identifer))
                    return _deviceSymmetric[identifer];
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return identifer;
        }

        public void AddOrUpdateDeviceSymmetric(string identifer, string password)
        {
            try
            {
                if (_deviceSymmetric.ContainsKey(identifer))
                    _deviceSymmetric[identifer] = password;
                else
                    _deviceSymmetric.Add(identifer, password);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
