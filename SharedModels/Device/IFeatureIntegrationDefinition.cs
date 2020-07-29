using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Device
{
    public enum IntegrationPoint
    {
        Desktop = 0,
        Mobile = 1
    }

    public interface IFeatureIntegrationDefinition
    {
        IntegrationPoint ControlIntegrationPoint { get; set; }
        IntegrationPoint FeatureIntegrationPoint { get; set; }
        string DesktopExecutable { get; set; }
    }
}
