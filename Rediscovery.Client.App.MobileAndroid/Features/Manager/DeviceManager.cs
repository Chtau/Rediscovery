using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.MobileAndroid.Features.Manager
{
    public sealed class DeviceManager
    {
        private DeviceManager()
        {
        }

        private static readonly Lazy<DeviceManager> lazy = new Lazy<DeviceManager>(() => new DeviceManager());

        public static DeviceManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public void Init()
        {
            try
            {
                // TODO: remove test data
                // Core.Database.Instance.Reset();
                
                Save(OnCreateMockDevice(new Guid("7AE08E8C-2C74-462C-950F-9EF82022A7B3"), "Device1", 1, new Guid("E4E7802E-07D1-4D99-A93C-04C4085F6B5E"), new Guid("C24FACAE-9368-4035-B5B3-5DB28C4A75B1"), new Guid("F24CA915-4297-4A22-AFBD-F4C4992E7FFF"), new Guid("279DD3C2-5E90-4A44-9569-15B719CC0867"), new Guid("6D15B5C4-4DB3-4B14-AD38-5EF0076F670A")), false);
                Save(OnCreateMockDevice(new Guid("E6BAAEB3-D92F-4FC1-85FE-92C4ED0F6644"), "Device2", 2, new Guid("5207AA48-99D5-43EA-9E60-439129A8328B"), new Guid("DE2A47C7-62C2-455F-B215-3D039FFB58BC"), new Guid("BC846F35-28BA-4B66-8A55-B187405BC312")), false);
                var devFav = OnCreateMockDevice(new Guid("C05A431E-4AD9-4F3B-A083-3B0365484684"), "Device3", 3, new Guid("0E2F4262-3E30-4126-B415-DDACA771BD1E"), new Guid("75482B34-73B5-4183-991B-1380D00CD449"), new Guid("7D663F65-5112-4605-BEB0-FE8C2A997BDB"), new Guid("8AB94500-351B-4364-BEAE-D9D2965CCA44"), new Guid("9D8CB391-4AA3-477F-9F22-17043B7D5D73"), new Guid("BB963E79-8762-4A66-8FC6-1D04B701C940"), new Guid("25C4B1E5-57DF-4E72-9854-86E455984125"));
                devFav.IsFavorite = true;
                Save(devFav, false);
                Save(OnCreateMockDevice(new Guid("8E26DEEE-1691-43A5-8752-F641D0AACCA0"), "Device4", 4, new Guid("85A450C4-DE90-420D-944B-A0E667FAD3C4"), new Guid("867BAB40-61A3-403B-BDFF-A3F958967BC4"), new Guid("DAA3E66F-2900-4B39-BC99-BB6F67CC9E67"), new Guid("D1706F5F-1A3C-49B4-8698-A954F9853F92")), false);
                
                OnUpdateOrderBy();
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        public Models.Device Get(Guid id)
        {
            try
            {
                return Core.Database.Instance.Get<Features.Models.Device>(x => x.DeviceId == id)?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
            return null;
        }

        public IEnumerable<Models.Device> GetAll()
        {
            try
            {
                return Core.Database.Instance.GetAll<Features.Models.Device>();
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
            return new List<Models.Device>();
        }

        public void Save(Features.Models.Device device, bool updateOrderBy = true)
        {
            try
            {
                if (device != null)
                {
                    var instance = Core.Database.Instance.Get<Features.Models.Device>(x => x.DeviceId == device.DeviceId)?.FirstOrDefault();
                    if (instance != null)
                    {
                        instance.Features = device.Features;
                        instance.IsFavorite = device.IsFavorite;
                        instance.Name = device.Name;
                        instance.OrderBy = device.OrderBy;
                        instance.ViewId = device.ViewId;
                        Core.Database.Instance.Update(instance);
                    } else
                    {
                        if (device.OrderBy <= 0)
                        {
                            var maxOrder = Core.Database.Instance.GetAll<Features.Models.Device>()?.OrderByDescending(x => x.OrderBy)?.Select(x => x.OrderBy)?.FirstOrDefault() ?? 0;
                            if (maxOrder >= 0)
                                device.OrderBy = ++maxOrder;
                        }
                        Core.Database.Instance.Insert(device);
                    }
                    if (updateOrderBy)
                        OnUpdateOrderBy();
                }
            } catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void OnUpdateOrderBy()
        {
            try
            {
                // update to bring favorites to the start and all other devices should be sorted by name
                int sortOrder = 1;
                var favorites = Core.Database.Instance.Get<Features.Models.Device>(x => x.IsFavorite).OrderBy(x => x.Name);
                if (favorites?.Count() > 0)
                {
                    foreach (var device in favorites)
                    {
                        device.OrderBy = sortOrder++;
                        Core.Database.Instance.Update(device);
                    }
                }
                var devices = Core.Database.Instance.Get<Features.Models.Device>(x => !x.IsFavorite).OrderBy(x => x.Name);
                if (devices?.Count() > 0)
                {
                    foreach (var device in devices)
                    {
                        device.OrderBy = sortOrder++;
                        Core.Database.Instance.Update(device);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private Features.Models.Device OnCreateMockDevice(Guid deviceId, string name, int viewId, params Guid[] features)
        {
            var device = new Features.Models.Device
            {
                DeviceId = deviceId,
                Name = name,
                OrderBy = 0,
                ViewId = viewId
            };
            if (features?.Length > 0)
            {
                device.Features = new System.Collections.Generic.List<Features.Models.Feature>();
                foreach (var item in features)
                {
                    device.Features.Add(new Features.Models.Feature
                    {
                        FeatureId = item,
                        Name = item.ToString()
                    });
                }
            }
            return device;
        }
    }
}