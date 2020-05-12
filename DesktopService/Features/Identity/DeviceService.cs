using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CommunicationResourceProvider;
using DesktopService.Features.Identity.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DesktopService.Features.Identity
{
    public class DeviceService : IDeviceService, IAuthenticateService
    {
        public const string DesktopHubRole = "desktophubconsumer";
        public const string InfoHubRole = "infohubconsumer";
        public const string DiscoveryServiceRole = "discoveryserviceconsumer";

        public event EventHandler<Device> NewDeviceAdded;
        public event EventHandler<DevicePendingAuthentication> NewDevicePendingAuthenticationAdded;

        private readonly Guid anonymouseId = new Guid("3D2AF409-6809-4ED1-B86A-451C94165E38");
        private readonly string anonymouseDeviceName = "Anonymous";

        private readonly ILogger<DeviceService> _logger;
        private readonly SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration _remoteResourceSettings;
        private readonly SharedConfigurations.DesktopService.Models.IdentityConfiguration _identitySettings;
        private readonly Random _random;
        private readonly DAL.IDBContext _dBContext;

        public DeviceService(ILoggerFactory loggerFactory, 
            IOptions<SharedConfigurations.DesktopService.Models.IdentityConfiguration> identitySettings,
            IOptions<SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration> remoteResourceSettings,
            DAL.IDBContext dBContext)
        {
            _logger = loggerFactory.CreateLogger<DeviceService>();
            _dBContext = dBContext;
            _identitySettings = identitySettings.Value;
            _remoteResourceSettings = remoteResourceSettings.Value;
            _random = new Random();
        }


        public string AuthenticateRemoteResourceConsumer(string consumerKey)
        {
            bool validKey = false;
            string roleName = null;
            if (string.Equals(_remoteResourceSettings.RediscoveryDesktopHubApplicationKey, consumerKey, StringComparison.OrdinalIgnoreCase))
            {
                validKey = true;
                roleName = DesktopHubRole;
            } else if (string.Equals(_remoteResourceSettings.RediscoveryDesktopInfoHubApplicationKey, consumerKey, StringComparison.OrdinalIgnoreCase))
            {
                validKey = true;
                roleName = InfoHubRole;
            } else if (string.Equals(_remoteResourceSettings.RediscoveryDiscoveryServiceApplicationKey, consumerKey, StringComparison.OrdinalIgnoreCase))
            {
                validKey = true;
                roleName = DiscoveryServiceRole;
            }
            if (validKey)
            {
                return OnCreateToken(new Claim[]
                {
                    new Claim(ClaimTypes.Sid, consumerKey),
                    new Claim(ClaimTypes.Name, consumerKey),
                    new Claim(ClaimTypes.Role, roleName)
                });
            }
            return null;
        }

        public async Task<Device> Authenticate(string deviceName, string passwordKey)
        {
            try
            {
                var user = await _dBContext.Instance.Table<Models.Device>().FirstOrDefaultAsync(x => x.DeviceName == deviceName && x.PasswordKey == passwordKey && x.PasswordKeyValidTill > DateTime.UtcNow);

                // return null if user not found
                if (user == null)
                {
                    if (_identitySettings.AnonymousLogin)
                    {
                        user = new Device
                        {
                            AllowAccess = true,
                            DeviceName = anonymouseDeviceName,
                            Id = anonymouseId,
                            PasswordKey = null,
                            PasswordKeyValidTill = DateTime.MaxValue,
                        };
                        user.Token = CreateNewToken(user.Id.ToString(), user.DeviceName);
                        return user;
                    }
                    return null;
                }


                user.AllowAccess = true; // update user db
                user.PasswordKeyValidTill = DateTime.MaxValue;

                // authentication successful so generate jwt token
                user.Token = CreateNewToken(user.Id.ToString(), user.DeviceName);

                await OnUpdateUser(user);

                // remove password before returning
                user.PasswordKey = null;

                return user;
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public string CreateNewToken(string sid, string name)
        {
            return OnCreateToken(new Claim[]
                {
                    new Claim(ClaimTypes.Sid, sid),
                    new Claim(ClaimTypes.Name, name),
                });
        }

        public async Task<IEnumerable<Device>> GetAll()
        {
            try
            {
                var users = await _dBContext.Instance.Table<Models.Device>().ToListAsync();

                return users.Select(x => {
                    x.PasswordKey = null;
                    return x;
                });
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<Device> GetById(Guid id)
        {
            try
            {
                var user = await _dBContext.Instance.Table<Models.Device>().FirstOrDefaultAsync(x => x.Id == id);
                if (user != null)
                    user.PasswordKey = null;

                return user;
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<Device> GetByName(string deviceName)
        {
            try
            {
#pragma warning disable RCS1155 // Use StringComparison when comparing strings.
                var user = await _dBContext.Instance.Table<Models.Device>().FirstOrDefaultAsync(x => x.DeviceName.ToLower() == deviceName.ToLower());
#pragma warning restore RCS1155 // Use StringComparison when comparing strings.
                if (user != null)
                    user.PasswordKey = null;
                else
                {
                    if (_identitySettings.AnonymousLogin)
                    {
                        user = new Device
                        {
                            AllowAccess = true,
                            DeviceName = anonymouseDeviceName,
                            Id = anonymouseId,
                            PasswordKey = null,
                            PasswordKeyValidTill = DateTime.MaxValue,
                        };
                    }
                }
                return user;
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<Device> AddDevice(string deviceName)
        {
            try
            {
                Device user = await GetByName(deviceName);
                if (user != null)
                {
                    user.Token = null;
                    if (user.PasswordKeyValidTill >= DateTime.UtcNow || string.IsNullOrWhiteSpace(user.PasswordKey))
                    {
                        user.PasswordKey = OnCreatePasswordKey();
                        user.PasswordKeyValidTill = DateTime.UtcNow.AddMinutes(5);
                    }
                    await OnUpdateUser(user);
                }
                else
                {
                    user = new Device
                    {
                        Id = Guid.NewGuid(),
                        PasswordKey = OnCreatePasswordKey(),
                        DeviceName = deviceName,
                        PasswordKeyValidTill = DateTime.UtcNow.AddMinutes(5)
                    };
                    await OnAddUser(user);
                }
                NewDeviceAdded?.Invoke(this, user);
                return user;
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        private string OnCreateToken(Claim[] claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_identitySettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(180),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task OnUpdateUser(Device device)
        {
            if (device.Id != anonymouseId)
                await _dBContext.Instance.UpdateAsync(device);
        }

        private async Task OnAddUser(Device device)
        {
            if (device.Id != anonymouseId)
                await _dBContext.Instance.InsertOrReplaceAsync(device);
        }

        private string OnCreatePasswordKey()
        {
            string retVal = "";
            for (int i = 0; i < _identitySettings.PasswordKeyLength; i++)
            {
                retVal += _random.Next(0, 9);
            }
            return retVal;
        }

        public string AuthenticateRemoteResourceConsumer(string consumerKey, string roleName)
        {
            bool validKey = false;
            if (string.Equals(_remoteResourceSettings.RediscoveryDesktopHubApplicationKey, consumerKey, StringComparison.OrdinalIgnoreCase))
            {
                validKey = true;
            }
            else if (string.Equals(_remoteResourceSettings.RediscoveryDesktopInfoHubApplicationKey, consumerKey, StringComparison.OrdinalIgnoreCase))
            {
                validKey = true;
            }
            else if (string.Equals(_remoteResourceSettings.RediscoveryDiscoveryServiceApplicationKey, consumerKey, StringComparison.OrdinalIgnoreCase))
            {
                validKey = true;
            }
            if (validKey)
            {
                return OnCreateToken(new Claim[]
                {
                    new Claim(ClaimTypes.Sid, consumerKey),
                    new Claim(ClaimTypes.Name, consumerKey),
                    new Claim(ClaimTypes.Role, roleName)
                });
            }
            return null;
        }

        public async Task<DevicePendingAuthentication> AddPendingAuthentication(string deviceName, string deviceIdentifier)
        {
            try
            {
                DevicePendingAuthentication devicePendingAuthentication = await PendingAuthenticationByIdentifier(deviceIdentifier);
                if (devicePendingAuthentication != null)
                {
                    devicePendingAuthentication.DeviceName = deviceName;
                    devicePendingAuthentication.RequestTime = DateTime.UtcNow;
                    await OnUpdatePendingAuthentication(devicePendingAuthentication);
                }
                else
                {
                    devicePendingAuthentication = new DevicePendingAuthentication
                    {
                        Id = Guid.NewGuid(),
                        DeviceName = deviceName,
                        DeviceIdentifier = deviceIdentifier,
                        RequestTime = DateTime.UtcNow
                    };
                    await OnAddPendingAuthentication(devicePendingAuthentication);
                }
                NewDevicePendingAuthenticationAdded?.Invoke(this, devicePendingAuthentication);
                return devicePendingAuthentication;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<DevicePendingAuthentication> PendingAuthenticationByIdentifier(string deviceIdentifier)
        {
            try
            {
                deviceIdentifier = deviceIdentifier.ToLower();
                return await _dBContext.Instance.Table<Models.DevicePendingAuthentication>().FirstOrDefaultAsync(x => x.DeviceIdentifier == deviceIdentifier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        private async Task<bool> OnUpdatePendingAuthentication(DevicePendingAuthentication devicePendingAuthentication)
        {
            try
            {
                await _dBContext.Instance.UpdateAsync(devicePendingAuthentication);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }

        private async Task<bool> OnAddPendingAuthentication(DevicePendingAuthentication devicePendingAuthentication)
        {
            try
            {
                await _dBContext.Instance.InsertOrReplaceAsync(devicePendingAuthentication);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }
    }
}
