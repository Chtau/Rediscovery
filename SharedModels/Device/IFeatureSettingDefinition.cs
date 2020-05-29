using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Device
{
    public interface IFeatureSettingDefinition
    {
        bool HasSettings { get; set; }
        bool SettingUIReadonly { get; set; }
        string SettingUIElementName { get; set; }
    }
}
