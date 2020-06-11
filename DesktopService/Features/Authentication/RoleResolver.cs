using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopService.Features.Authentication
{
    public class RoleResolver : IRoleResolver
    {
        private readonly ILogger<RoleResolver> _logger;
        private readonly SharedConfigurations.DesktopService.Models.RoleConfiguration _roleConfiguration;

        public RoleResolver(ILoggerFactory loggerFactory,
            IOptions<SharedConfigurations.DesktopService.Models.RoleConfiguration> roleOptions)
        {
            _logger = loggerFactory.CreateLogger<RoleResolver>();
            _roleConfiguration = roleOptions.Value;
        }

        public string GetRole(string deviceIdentifier)
        {
            try
            {
                if (_roleConfiguration.ResourceConsumers?.Contains(deviceIdentifier) == true)
                    return _roleConfiguration.ResourceConsumerRoleName;
            } catch (Exception ex)
            {
                _logger.LogError(ex, $"Resolve Role for Device failed (DeviceIdentifier:{deviceIdentifier})");
            }
            return _roleConfiguration.DeviceRoleName;
        }
    }
}
