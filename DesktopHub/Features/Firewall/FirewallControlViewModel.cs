using Avalonia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace DesktopHub.Features.Firewall
{
    public class FirewallControlViewModel : BaseViewModel
    {
        private readonly List<SharedConfigurations.Hub.Models.FirewallRulesConfiguration> _firewallRulesConfiguration;
        private readonly SharedConfigurations.Hub.Models.HubConfiguration _hubConfiguration;

        public ObservableCollection<FirewallRuleViewModel> Items { get; set; } = new ObservableCollection<FirewallRuleViewModel>();

        public FirewallControlViewModel()
        {
            _hubConfiguration = Program.Configuration.GetSection(SharedConfigurations.Hub.Models.HubConfiguration.SectionName).Get<SharedConfigurations.Hub.Models.HubConfiguration>();
            _firewallRulesConfiguration = Program.Configuration.GetSection(SharedConfigurations.Hub.Models.FirewallRulesConfiguration.SectionName).Get<List<SharedConfigurations.Hub.Models.FirewallRulesConfiguration>>();
            if (_firewallRulesConfiguration?.Count > 0)
            {
                foreach (var item in _firewallRulesConfiguration)
                {
                    Items.Add(new FirewallRuleViewModel
                    {
                        ExePath = item.ExePath,
                        RuleName = item.RuleName,
                        RuleSet = OnFWIsActive(item.RuleName)
                    });
                }
            }
        }

        private bool OnFWIsActive(string ruleName)
        {
            return SharedFeatureFunctions.FirewallRule.RuleExists(ruleName) == SharedFeatureFunctions.FirewallRule.RuleState.True;
        }

        public void TrySetFW(FirewallRuleViewModel item)
        {
            if (item != null)
            {
                Elevate(_hubConfiguration.FirewallApp, $"--addfw --name:{item.RuleName} --exepath:\"{item.ExePath}\"");
            }
        }

        private void Elevate(string filePath, string parameters)
        {
            var SelfProc = new ProcessStartInfo
            {
                UseShellExecute = true,
                //WorkingDirectory = Environment.CurrentDirectory,
                FileName = filePath,
                Arguments = parameters,
                Verb = "runas"
            };
            try
            {
                Process.Start(SelfProc);
            }
            catch
            {
                System.Diagnostics.Debug.Print("Unable to elevate!" + Environment.NewLine);
            }
        }
    }
}
