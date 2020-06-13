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
        public event EventHandler<PluginExchangeEntity<PluginFeatureData>> SendData;

        public PluginExchangeEntity<FeatureState> FeatureStateChange(PluginExchangeEntity<FeatureState> featureStateChange)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[FeatureStateChange]: {featureStateChange.Entity.CurrentState}");
            Console.ResetColor();
            return new PluginExchangeEntity<FeatureState>
            {
                Sid = featureStateChange.Sid,
                Entity = new FeatureState
                {
                    CurrentState = featureStateChange.Entity.CurrentState,
                    FeatureId = featureStateChange.Entity.FeatureId
                }
            };
        }

        public List<PluginFeatureProfil> GetFeatureProfiles(Guid featureId)
        {
            return new List<PluginFeatureProfil>();
        }

        public PluginFeatureSetting GetFeatureSettings(Guid featureId)
        {
            return new PluginFeatureSetting();
        }

        public byte[] GetFeatureUIArchive(Guid featureId)
        {
            return new byte[0];
        }

        public void ReceivedData(PluginExchangeEntity<PluginFeatureData> deviceFeatureData)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[ReceivedData]: {deviceFeatureData.Entity.Data}");
            Console.ResetColor();

            Task.Run(async () =>
            {
                await Task.Delay(200);
                Console.WriteLine($"Send [DeviceFeatureData] response to Client");
                SendData.Invoke(this, new PluginExchangeEntity<PluginFeatureData>
                {
                    Sid = deviceFeatureData.Sid,
                    Entity = new PluginFeatureData(deviceFeatureData.Entity.DeviceId, deviceFeatureData.Entity.FeatureId,
                        deviceFeatureData.Entity.ProfileId, $"{DateTime.Now} Service send data")
                });
            });
        }
    }
}
