using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DesktopService.Features.Identity.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DesktopService.Features.Identity
{
    public class DeviceService : IDeviceService
    {
        public event EventHandler<Device> NewDeviceAdded;

        private readonly ILogger<DeviceService> _logger;
        private readonly SharedConfigurations.DesktopService.Models.IdentityConfiguration _identitySettings;
        private readonly Random _random;
        private readonly DAL.IDBContext _dBContext;

        public DeviceService(ILoggerFactory loggerFactory, IOptions<SharedConfigurations.DesktopService.Models.IdentityConfiguration> identitySettings, DAL.IDBContext dBContext)
        {
            _logger = loggerFactory.CreateLogger<DeviceService>();
            _dBContext = dBContext;
            _identitySettings = identitySettings.Value;
            _random = new Random();
        }


        public async Task<Device> Authenticate(string deviceName, string passwordKey)
        {
            try
            {
                var user = await _dBContext.Instance.Table<Models.Device>().FirstOrDefaultAsync(x => x.DeviceName == deviceName && x.PasswordKey == passwordKey && x.PasswordKeyValidTill > DateTime.UtcNow);

                // return null if user not found
                if (user == null)
                    return null;

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
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_identitySettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Sid, sid),
                    new Claim(ClaimTypes.Name, name),
                }),
                Expires = DateTime.UtcNow.AddDays(180),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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

        private async Task OnUpdateUser(Device device)
        {
            await _dBContext.Instance.UpdateAsync(device);
        }

        private async Task OnAddUser(Device device)
        {
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
    }
}
