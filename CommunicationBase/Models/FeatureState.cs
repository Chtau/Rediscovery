using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Base.Models
{
    public class FeatureState
    {
        public enum State
        {
            Unknown = 0,
            Start = 1,
            Stop = 2,
            Error = 3
        }

        public Guid FeatureId { get; set; }

        public State CurrentState { get; set; }
    }
}
