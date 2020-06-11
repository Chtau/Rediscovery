using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery
{
    public static class EnumExtensions
    {
        public static CommunicationBase.ConnectionState ConvertToCommunicationEnum(this SharedBase.Connection.Enums.ConnectionState connectionState)
        {
            switch (connectionState)
            {
                case SharedBase.Connection.Enums.ConnectionState.None:
                    return CommunicationBase.ConnectionState.None;
                case SharedBase.Connection.Enums.ConnectionState.OK:
                    return CommunicationBase.ConnectionState.OK;
                case SharedBase.Connection.Enums.ConnectionState.Error:
                    return CommunicationBase.ConnectionState.Error;
                case SharedBase.Connection.Enums.ConnectionState.Warning:
                    return CommunicationBase.ConnectionState.Warning;
                case SharedBase.Connection.Enums.ConnectionState.Offline:
                    return CommunicationBase.ConnectionState.Offline;
                case SharedBase.Connection.Enums.ConnectionState.Denied:
                    return CommunicationBase.ConnectionState.Denied;
                case SharedBase.Connection.Enums.ConnectionState.WaitForApprovel:
                    return CommunicationBase.ConnectionState.WaitForApprovel;
                default:
                    return CommunicationBase.ConnectionState.None;
            }
        }

        public static SharedBase.Connection.Enums.ConnectionState ConvertToSharedCoreEnum(this CommunicationBase.ConnectionState connectionState)
        {
            switch (connectionState)
            {
                case CommunicationBase.ConnectionState.None:
                    return SharedBase.Connection.Enums.ConnectionState.None;
                case CommunicationBase.ConnectionState.OK:
                    return SharedBase.Connection.Enums.ConnectionState.OK;
                case CommunicationBase.ConnectionState.Error:
                    return SharedBase.Connection.Enums.ConnectionState.Error;
                case CommunicationBase.ConnectionState.Warning:
                    return SharedBase.Connection.Enums.ConnectionState.Warning;
                case CommunicationBase.ConnectionState.Offline:
                    return SharedBase.Connection.Enums.ConnectionState.Offline;
                case CommunicationBase.ConnectionState.Denied:
                    return SharedBase.Connection.Enums.ConnectionState.Denied;
                case CommunicationBase.ConnectionState.WaitForApprovel:
                    return SharedBase.Connection.Enums.ConnectionState.WaitForApprovel;
                default:
                    return SharedBase.Connection.Enums.ConnectionState.None;
            }
        }
    }
}
