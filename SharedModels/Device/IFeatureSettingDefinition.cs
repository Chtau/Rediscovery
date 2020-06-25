using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Device
{
    public interface IFeatureSettingDefinition
    {
        bool HasSettingConfiguration { get; set; }
    }
}
