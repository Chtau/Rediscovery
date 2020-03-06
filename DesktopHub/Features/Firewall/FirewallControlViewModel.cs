using Avalonia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Linq;

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
                Elevate(_hubConfiguration.FirewallApp, 
                    $"{SharedCommandArguments.Firewall.Arguments.CommandAddFirewall} {SharedCommandArguments.Firewall.Arguments.CommandRuleName}:{item.RuleName} {SharedCommandArguments.Firewall.Arguments.CommandRuleExePath}:\"{item.ExePath}\"",
                    () =>
                    {
                        var rule = Items.FirstOrDefault(x => string.Equals(x.RuleName, item.RuleName, StringComparison.OrdinalIgnoreCase));
                        if (rule != null)
                            rule.RuleSet = OnFWIsActive(item.RuleName);
                    });
            }
        }

        private void Elevate(string filePath, string parameters, Action exitCallback)
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
                var prc = Process.Start(SelfProc);
                prc.Exited += (object sender, EventArgs e) =>
                {
                    exitCallback?.Invoke();
                };
            }
            catch
            {
                System.Diagnostics.Debug.Print("Unable to elevate!" + Environment.NewLine);
            }
        }
    }
}
