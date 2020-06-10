using CommunicationBase.Models;
using CommunicationFeatureProvider;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcTestService.DummyImpl
{
    public class FeatureManager : IFeatureManager
    {
        public event EventHandler<ExchangeEntity<DeviceFeatureData>> SendData;

        public ExchangeEntity<FeatureState> FeatureStateChange(ExchangeEntity<FeatureState> featureStateChange)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[FeatureStateChange]: {featureStateChange.Entity.CurrentState}");
            Console.ResetColor();
            return new ExchangeEntity<FeatureState>
            {
                Sid = featureStateChange.Sid,
                Entity = new FeatureState
                {
                    CurrentState = featureStateChange.Entity.CurrentState,
                    FeatureId = featureStateChange.Entity.FeatureId
                }
            };
        }

        public List<DeviceFeatureProfil> GetFeatureProfiles(Guid featureId)
        {
            return new List<DeviceFeatureProfil>();
        }

        public DeviceFeatureSetting GetFeatureSettings(Guid featureId)
        {
            return new DeviceFeatureSetting();
        }

        public byte[] GetFeatureUIArchive(Guid featureId)
        {
            return new byte[0];
        }

        public void ReceivedData(ExchangeEntity<DeviceFeatureData> deviceFeatureData)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[ReceivedData]: {deviceFeatureData.Entity.Data}");
            Console.ResetColor();

            Task.Run(async () =>
            {
                await Task.Delay(200);
                Console.WriteLine($"Send [DeviceFeatureData] response to Client");
                SendData.Invoke(this, new ExchangeEntity<DeviceFeatureData>
                {
                    Sid = deviceFeatureData.Sid,
                    Entity = new DeviceFeatureData(deviceFeatureData.Entity.DeviceId, deviceFeatureData.Entity.FeatureId,
                        deviceFeatureData.Entity.ProfileId, $"{DateTime.Now} Service send data")
                });
            });
        }
    }
}
