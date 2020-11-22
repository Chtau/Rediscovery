using SharedFeatureFunctions;
using System;
using System.Linq;
using static SharedFeatureFunctions.FirewallRule;

namespace Rediscovery.Client.App.Firewall.Console
{
    static class Program
    {
        static void Main(string[] args)
        {
            short rulePort = -1;
            string ruleName = null;
            string exePath = null;
            ProtocolType protocolType = ProtocolType.Any;
            if (args.Any(x => x.StartsWith(SharedCommandArguments.Firewall.Arguments.CommandRulePort, StringComparison.OrdinalIgnoreCase)))
            {
                var valueArg = args.First(x => x.StartsWith(SharedCommandArguments.Firewall.Arguments.CommandRulePort, StringComparison.OrdinalIgnoreCase));
                var vals = valueArg.Split(':');
                if (short.TryParse(vals[1].Trim(), out short port))
                    rulePort = port;
            }
            if (args.Any(x => x.StartsWith(SharedCommandArguments.Firewall.Arguments.CommandRuleName, StringComparison.OrdinalIgnoreCase)))
            {
                var valueArg = args.First(x => x.StartsWith(SharedCommandArguments.Firewall.Arguments.CommandRuleName, StringComparison.OrdinalIgnoreCase));
                var vals = valueArg.Split(':');
                ruleName = vals[1].Trim();
            }
            if (args.Any(x => x.StartsWith(SharedCommandArguments.Firewall.Arguments.CommandRuleExePath, StringComparison.OrdinalIgnoreCase)))
            {
                var valueArg = args.First(x => x.StartsWith(SharedCommandArguments.Firewall.Arguments.CommandRuleExePath, StringComparison.OrdinalIgnoreCase));
                var vals = valueArg.Split(':');
                exePath = vals[1].Trim();
            }
            if (args.Any(x => x.StartsWith(SharedCommandArguments.Firewall.Arguments.CommandRuleType, StringComparison.OrdinalIgnoreCase)))
            {
                var valueArg = args.First(x => x.StartsWith(SharedCommandArguments.Firewall.Arguments.CommandRuleType, StringComparison.OrdinalIgnoreCase));
                var vals = valueArg.Split(':');
                if (Enum.TryParse<ProtocolType>(vals[1].Trim(), true, out ProtocolType type))
                    protocolType = type;
            }
            if (args.Any(x => x.StartsWith("?", StringComparison.OrdinalIgnoreCase) || x.StartsWith("help", StringComparison.OrdinalIgnoreCase)))
            {
                System.Console.WriteLine("Help for Desktop Firewall");
                System.Console.WriteLine("Arguments");
                System.Console.WriteLine($"    {SharedCommandArguments.Firewall.Arguments.CommandAddFirewall}    \"Creates Windows Firewall Rule\"");
                System.Console.WriteLine($"    {SharedCommandArguments.Firewall.Arguments.CommandRemoveFirewall}    \"Removes Windows Firewall Rule\"");
                System.Console.WriteLine($"    {SharedCommandArguments.Firewall.Arguments.CommandRuleName}    \"Name of the Firewall rule\"");
                System.Console.WriteLine($"    {SharedCommandArguments.Firewall.Arguments.CommandRulePort}    \"Port for the Firewall rule\"");
                System.Console.WriteLine($"    {SharedCommandArguments.Firewall.Arguments.CommandRuleType}    \"Firewall rule type\" ({string.Join(',', Enum.GetNames(typeof(ProtocolType)))})");
                System.Console.WriteLine($"    {SharedCommandArguments.Firewall.Arguments.CommandRuleExePath}    \"App execution path for the Firewall rule\"");
            }
            else if (args.Any(x => x.StartsWith(SharedCommandArguments.Firewall.Arguments.CommandAddFirewall, StringComparison.OrdinalIgnoreCase)))
            {
                if (FirewallRule.RuleExists(ruleName, rulePort) != RuleState.True)
                {
                    RuleState result = RuleState.False;
                    if (!string.IsNullOrWhiteSpace(exePath))
                    {
                        result = FirewallRule.RuleCreate(ruleName, exePath);
                    } else
                    {
                        result = FirewallRule.RuleCreate((ushort)rulePort, ruleName, protocolType);
                    }
                    if (result == RuleState.True)
                    {
                        System.Console.WriteLine("Firewall rule created");
                    }
                    else
                    {
                        System.Console.ForegroundColor = ConsoleColor.Red;
                        System.Console.WriteLine("Could not create Firewall rule (Restart with Administrator rights)");
                        System.Console.WriteLine("Alternative you can create the Rule with the Command: " + FirewallRule.GetFWRuleCreate(rulePort, ruleName, protocolType));
                        System.Console.ResetColor();
                    }
                }
                else
                {
                    System.Console.WriteLine("Firewall rule already exists");
                }
            }
            else if (args.Any(x => x.StartsWith(SharedCommandArguments.Firewall.Arguments.CommandRemoveFirewall, StringComparison.OrdinalIgnoreCase)))
            {
                if (FirewallRule.RuleExists(ruleName, rulePort) == RuleState.True)
                {
                    if (FirewallRule.RuleDelete(ruleName) == RuleState.True)
                    {
                        System.Console.WriteLine("Firewall rule removed");
                    }
                    else
                    {
                        System.Console.ForegroundColor = ConsoleColor.Red;
                        System.Console.WriteLine("Could not remove Firewall rule (Restart with Administrator rights)");
                        System.Console.WriteLine("Alternative you can create the Rule with the Command: " + FirewallRule.GetFWRuleDelete(rulePort, ruleName, protocolType));
                        System.Console.ResetColor();
                    }
                }
                else
                {
                    System.Console.WriteLine("Firewall rule already removed");
                }
            } else
            {
                System.Console.WriteLine("No valid command");
            }
            //Console.ReadKey();
        }
    }
}
