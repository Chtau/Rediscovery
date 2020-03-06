using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCommandArguments.Firewall
{
    public static class Arguments
    {
        public const string CommandAddFirewall = "--addfw";
        public const string CommandRemoveFirewall = "--removefw";
        public const string CommandRuleName = "--name:";
        public const string CommandRulePort = "--port:";
        public const string CommandRuleType = "--type:";
        public const string CommandRuleExePath = "--exepath:";
    }
}
