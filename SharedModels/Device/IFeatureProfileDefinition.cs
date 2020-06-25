using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Device
{
    public interface IFeatureProfileDefinition
    {
        bool HasProfilConfiguration { get; set; }
    }
}
