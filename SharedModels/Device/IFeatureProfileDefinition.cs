using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Device
{
    public interface IFeatureProfileDefinition
    {
        bool HasProfiles { get; set; }
        bool ProfileUIReadonly { get; set; }
        string ProfileUIElementName { get; set; }
    }
}
