using CommunicationBase.Models;
using CommunicationFeatureProvider;
using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcTestService.DummyImpl
{
    public class FeatureManager : IFeatureManager
    {
        public event EventHandler<ExchangeEntity<FeatureData>> SendData;

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

        public List<FeatureProfil> GetFeatureProfiles(Guid featureId)
        {
            return new List<FeatureProfil>();
        }

        public FeatureSetting GetFeatureSettings(Guid featureId)
        {
            return new FeatureSetting();
        }

        public byte[] GetFeatureUIArchive(Guid featureId)
        {
            return new byte[0];
        }

        public void ReceivedData(ExchangeEntity<FeatureData> deviceFeatureData)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[ReceivedData]: {deviceFeatureData.Entity.Data}");
            Console.ResetColor();

            Task.Run(async () =>
            {
                await Task.Delay(200);
                Console.WriteLine($"Send [DeviceFeatureData] response to Client");
                SendData.Invoke(this, new ExchangeEntity<FeatureData>
                {
                    Sid = deviceFeatureData.Sid,
                    Entity = new FeatureData(deviceFeatureData.Entity.DeviceId, deviceFeatureData.Entity.FeatureId,
                        deviceFeatureData.Entity.ProfileId, $"{DateTime.Now} Service send data")
                });
            });
        }
    }
}
