using DALDesktopService.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALDesktopService.Repository
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly IDBContext _dBContext;
        private readonly ILogger<DeviceRepository> _logger;

        public event EventHandler<Device> DeviceChanged;
        public event EventHandler<Guid> DeviceDeleted;


        public DeviceRepository(IDBContext dBContext, ILoggerFactory loggerFactory)
        {
            _dBContext = dBContext;
            _logger = loggerFactory.CreateLogger<DeviceRepository>();
        }

        public async Task<bool> DeleteDevice(Guid id)
        {
            try
            {
                await _dBContext.Instance.Table<Models.Device>().DeleteAsync(x => x.Id == id);
                DeviceDeleted?.Invoke(this, id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }

        public async Task<IEnumerable<Device>> GetAll()
        {
            try
            {
                return await _dBContext.Instance.Table<Models.Device>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<Device> GetByDeviceIdentifier(string deviceIdentifier)
        {
            try
            {
                deviceIdentifier = deviceIdentifier.ToLower();
                return await _dBContext.Instance.Table<Models.Device>().FirstOrDefaultAsync(x => x.DeviceIdentifier == deviceIdentifier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<Device> GetById(Guid id)
        {
            try
            {
                return await _dBContext.Instance.Table<Models.Device>().FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<Device> SaveDevice(Device device)
        {
            try
            {
                Device device1 = await GetByDeviceIdentifier(device.DeviceIdentifier);
                if (device1 != null)
                {
                    device1.Role = device.Role;
                    device1.DeviceName = device.DeviceName;
                    device1.AllowAccess = device.AllowAccess;
                    DeviceMetadata.UpdateInstance(device, device1);
                    await _dBContext.Instance.UpdateAsync(device1);
                }
                else
                {
                    device1 = new Device
                    {
                        Id = Guid.NewGuid(),
                        DeviceName = device.DeviceName,
                        AllowAccess = device.AllowAccess,
                        DeviceIdentifier = device.DeviceIdentifier,
                        Role = device.Role,
                    };
                    DeviceMetadata.UpdateInstance(device, device1);
                    await _dBContext.Instance.InsertOrReplaceAsync(device1);
                }
                DeviceChanged?.Invoke(this, device1);
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
