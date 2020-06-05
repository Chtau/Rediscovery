using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationBase.Models
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

        public string FeatureId { get; set; }

        public State CurrentState { get; set; }
    }
}
