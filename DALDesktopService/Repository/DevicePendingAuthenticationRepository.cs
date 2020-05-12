using DALDesktopService.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DALDesktopService.Repository
{
    public class DevicePendingAuthenticationRepository : IDevicePendingAuthenticationRepository
    {
        private readonly IDBContext _dBContext;
        private readonly ILogger<DevicePendingAuthenticationRepository> _logger;

        public event EventHandler<DevicePendingAuthentication> DevicePendingAuthenticationChanged;
        public event EventHandler<Guid> DevicePendingAuthenticationDeleted;

        public DevicePendingAuthenticationRepository(IDBContext dBContext, ILoggerFactory loggerFactory)
        {
            _dBContext = dBContext;
            _logger = loggerFactory.CreateLogger<DevicePendingAuthenticationRepository>();
        }

        public async Task<bool> DeleteDevicePendingAuthentication(Guid id)
        {
            try
            {
                await _dBContext.Instance.Table<Models.DevicePendingAuthentication>().DeleteAsync(x => x.Id == id);
                DevicePendingAuthenticationDeleted?.Invoke(this, id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }

        public async Task<IEnumerable<DevicePendingAuthentication>> GetAll()
        {
            try
            {
                return await _dBContext.Instance.Table<Models.DevicePendingAuthentication>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<DevicePendingAuthentication> GetByDeviceIdentifier(string deviceIdentifier)
        {
            try
            {
                deviceIdentifier = deviceIdentifier.ToLower();
                return await _dBContext.Instance.Table<Models.DevicePendingAuthentication>().FirstOrDefaultAsync(x => x.DeviceName == deviceIdentifier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<DevicePendingAuthentication> GetById(Guid id)
        {
            try
            {
                return await _dBContext.Instance.Table<Models.DevicePendingAuthentication>().FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<DevicePendingAuthentication> SaveDevicePendingAuthentication(DevicePendingAuthentication devicePendingAuthentication)
        {
            try
            {
                DevicePendingAuthentication device1 = await GetByDeviceIdentifier(devicePendingAuthentication.DeviceIdentifier);
                if (device1 != null)
                {
                    device1.DeviceName = devicePendingAuthentication.DeviceName;
                    device1.RequestTime = devicePendingAuthentication.RequestTime;
                    DeviceMetadata.UpdateInstance(devicePendingAuthentication, device1);
                    await _dBContext.Instance.UpdateAsync(device1);
                }
                else
                {
                    device1 = new DevicePendingAuthentication
                    {
                        Id = Guid.NewGuid(),
                        DeviceName = devicePendingAuthentication.DeviceName,
                        RequestTime = devicePendingAuthentication.RequestTime,
                        DeviceIdentifier = devicePendingAuthentication.DeviceIdentifier,
                    };
                    DeviceMetadata.UpdateInstance(devicePendingAuthentication, device1);
                    await _dBContext.Instance.InsertOrReplaceAsync(device1);
                }
                DevicePendingAuthenticationChanged?.Invoke(this, device1);
                return device1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }
    }
}
