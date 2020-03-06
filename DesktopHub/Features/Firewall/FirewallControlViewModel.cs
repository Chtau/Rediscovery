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

        public ObservableCollection<FirewallRuleViewModel> Items { get; set; } = new ObservableCollection<FirewallRuleViewModel>();

        public FirewallControlViewModel()
        {
            _firewallRulesConfiguration = Program.Configuration.GetSection(SharedConfigurations.Hub.Models.FirewallRulesConfiguration.SectionName).Get<List<SharedConfigurations.Hub.Models.FirewallRulesConfiguration>>();
            if (_firewallRulesConfiguration?.Count > 0)
            {
                foreach (var item in _firewallRulesConfiguration)
                {
                    Items.Add(new FirewallRuleViewModel
                    {
                        ExePath = item.ExePath,
                        RuleName = item.RuleName
                    });
                }
            }
        }

        public void TrySetFW(FirewallRuleViewModel item)
        {
            if (item != null)
            {
                
            }
        }

        private static void Elevate(string filePath, string parameters)
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
