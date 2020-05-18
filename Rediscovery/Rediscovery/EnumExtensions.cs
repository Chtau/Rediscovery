using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery
{
    public static class EnumExtensions
    {
        public static CommunicationBase.ConnectionState ConvertToCommunicationEnum(this SharedCoreModels.Enums.ConnectionState connectionState)
        {
            switch (connectionState)
            {
                case SharedCoreModels.Enums.ConnectionState.None:
                    return CommunicationBase.ConnectionState.None;
                case SharedCoreModels.Enums.ConnectionState.OK:
                    return CommunicationBase.ConnectionState.OK;
                case SharedCoreModels.Enums.ConnectionState.Error:
                    return CommunicationBase.ConnectionState.Error;
                case SharedCoreModels.Enums.ConnectionState.Warning:
                    return CommunicationBase.ConnectionState.Warning;
                case SharedCoreModels.Enums.ConnectionState.Offline:
                    return CommunicationBase.ConnectionState.Offline;
                case SharedCoreModels.Enums.ConnectionState.Denied:
                    return CommunicationBase.ConnectionState.Denied;
                case SharedCoreModels.Enums.ConnectionState.WaitForApprovel:
                    return CommunicationBase.ConnectionState.WaitForApprovel;
                default:
                    return CommunicationBase.ConnectionState.None;
            }
        }

        public static SharedCoreModels.Enums.ConnectionState ConvertToSharedCoreEnum(this CommunicationBase.ConnectionState connectionState)
        {
            switch (connectionState)
            {
                case CommunicationBase.ConnectionState.None:
                    return SharedCoreModels.Enums.ConnectionState.None;
                case CommunicationBase.ConnectionState.OK:
                    return SharedCoreModels.Enums.ConnectionState.OK;
                case CommunicationBase.ConnectionState.Error:
                    return SharedCoreModels.Enums.ConnectionState.Error;
                case CommunicationBase.ConnectionState.Warning:
                    return SharedCoreModels.Enums.ConnectionState.Warning;
                case CommunicationBase.ConnectionState.Offline:
                    return SharedCoreModels.Enums.ConnectionState.Offline;
                case CommunicationBase.ConnectionState.Denied:
                    return SharedCoreModels.Enums.ConnectionState.Denied;
                case CommunicationBase.ConnectionState.WaitForApprovel:
                    return SharedCoreModels.Enums.ConnectionState.WaitForApprovel;
                default:
                    return SharedCoreModels.Enums.ConnectionState.None;
            }
        }
    }
}
