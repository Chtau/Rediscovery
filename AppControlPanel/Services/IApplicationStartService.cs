using System;
using System.Collections.Generic;
using System.Text;

namespace AppControlPanel.Services
{
    public interface IApplicationStartService
    {
        bool Start(SharedConfigurations.AppControlPanel.Models.AppViewModel appViewModel);
    }
}
