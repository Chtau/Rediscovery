using SharedFeatureFunctions;
using System;
using System.Linq;
using static SharedFeatureFunctions.FirewallRule;

namespace DesktopFirewall
{
    class Program
    {
        private const string CommandAddFirewall = "--addfw";
        private const string CommandRemoveFirewall = "--removefw";
        private const string CommandRuleName = "--name:";
        private const string CommandRulePort = "--port:";
        private const string CommandRuleType = "--type:";

        static void Main(string[] args)
        {
            ushort rulePort = 8888;
            string ruleName = null;
            ProtocolType protocolType = ProtocolType.Any;
            if (args.Any(x => x.StartsWith(CommandRulePort, StringComparison.OrdinalIgnoreCase)))
            {
                var valueArg = args.First(x => x.StartsWith(CommandRulePort, StringComparison.OrdinalIgnoreCase));
                var vals = valueArg.Split(':');
                if (ushort.TryParse(vals[1].Trim(), out ushort port))
                    rulePort = port;
            }
            if (args.Any(x => x.StartsWith(CommandRuleName, StringComparison.OrdinalIgnoreCase)))
            {
                var valueArg = args.First(x => x.StartsWith(CommandRuleName, StringComparison.OrdinalIgnoreCase));
                var vals = valueArg.Split(':');
                ruleName = vals[1].Trim();
            }
            if (args.Any(x => x.StartsWith(CommandRuleType, StringComparison.OrdinalIgnoreCase)))
            {
                var valueArg = args.First(x => x.StartsWith(CommandRuleType, StringComparison.OrdinalIgnoreCase));
                var vals = valueArg.Split(':');
                if (Enum.TryParse<ProtocolType>(vals[1].Trim(), true, out ProtocolType type))
                    protocolType = type;
            }
            if (args.Any(x => x.StartsWith("?", StringComparison.OrdinalIgnoreCase) || x.StartsWith("help", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Help for Desktop Firewall");
                Console.WriteLine("Arguments");
                Console.WriteLine($"    {CommandAddFirewall}    \"Creates Windows Firewall Rule\"");
                Console.WriteLine($"    {CommandRemoveFirewall}    \"Removes Windows Firewall Rule\"");
                Console.WriteLine($"    {CommandRuleName}    \"Name of the Firewall rule\"");
                Console.WriteLine($"    {CommandRulePort}    \"Port for the Firewall rule\"");
                Console.WriteLine($"    {CommandRuleType}    \"Firewall rule type\" ({string.Join(',', Enum.GetNames(typeof(ProtocolType)))})");
            }
            else if (args.Any(x => x.StartsWith(CommandAddFirewall, StringComparison.OrdinalIgnoreCase)))
            {
                if (FirewallRule.RuleExists(ruleName, rulePort) != RuleState.True)
                {
                    if (FirewallRule.RuleCreate(rulePort, ruleName, protocolType) == RuleState.True)
                    {
                        Console.WriteLine("Firewall rule created");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Could not create Firewall rule (Restart with Administrator rights)");
                        Console.WriteLine("Alternative you can create the Rule with the Command: " + FirewallRule.GetFWRuleCreate(rulePort, ruleName, protocolType));
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.WriteLine("Firewall rule already exists");
                }
            }
            else if (args.Any(x => x.StartsWith(CommandRemoveFirewall, StringComparison.OrdinalIgnoreCase)))
            {
                if (FirewallRule.RuleExists(ruleName, rulePort) == RuleState.True)
                {
                    if (FirewallRule.RuleDelete(ruleName) == RuleState.True)
                    {
                        Console.WriteLine("Firewall rule removed");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Could not remove Firewall rule (Restart with Administrator rights)");
                        Console.WriteLine("Alternative you can create the Rule with the Command: " + FirewallRule.GetFWRuleDelete(rulePort, ruleName, protocolType));
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.WriteLine("Firewall rule already removed");
                }
            } else
            {
                Console.WriteLine("No valid command");
            }
        }
    }
}
