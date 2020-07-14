using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationHeartbeatProvider
{
    public interface IHeartbeatStatistic
    {
        event EventHandler<Dictionary<string, List<HeartbeatResult>>> UpdatedHeartbeatStatics;

        void NewBeat(HeartbeatResult heartbeatResult);
        Dictionary<string, List<HeartbeatResult>> Get();
    }
}
