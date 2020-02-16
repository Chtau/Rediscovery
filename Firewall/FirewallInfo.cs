using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Firewall
{
    public class FirewallInfo
    {
        public class FirewallCommandResult
        {
            public bool Result { get; set; }

            public bool RequiredAdministratorRights { get; set; }

            public FirewallCommandResult(bool result, bool requiredAdministratorRights)
            {
                Result = result;
                RequiredAdministratorRights = requiredAdministratorRights;
            }
        }

        private const string FirewallBaseCommand = "netsh advfirewall firewall ";
        private const string FirewallShowRuleCommand = "show rule name=\"{0}\"";


        public FirewallCommandResult RuleExists(string ruleName)
        {
            string cmd = FirewallBaseCommand + string.Format(FirewallShowRuleCommand, ruleName);
            return OnSendCommand(cmd);
        }

        private FirewallCommandResult OnSendCommand(string cmd)
        {
            bool result = false;
            bool hasResult = false;
            bool reqAdmin = false;
            string lastOutput = "1";
            Task.Run(() =>
            {
                var prc = OnGetProcess((object sender, DataReceivedEventArgs e) =>
                {
                    if (!hasResult)
                    {
                        System.Diagnostics.Debug.Print(e.Data);
                        if (string.Equals(e.Data, "OK.", StringComparison.OrdinalIgnoreCase))
                        {
                            result = true;
                            hasResult = true;
                        }
                        if (e.Data.IndexOf("ADMIN", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            result = false;
                            reqAdmin = true;
                            hasResult = true;
                        }
                        // cmd finishes commands with double empty lines
                        if (e.Data == "" && lastOutput == "")
                        {
                            hasResult = true;
                        }
                        lastOutput = e.Data;
                    }
                });
                prc.StandardInput.WriteLine(cmd + Environment.NewLine);
            });

            do
            {
                System.Threading.Thread.Sleep(10);
            } while (!hasResult);
            return new FirewallCommandResult(result, reqAdmin);
        }

        private Process OnGetProcess(DataReceivedEventHandler eventHandlerConsoleOutput)
        {
            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.OutputDataReceived += eventHandlerConsoleOutput;
            process.Start();
            process.BeginOutputReadLine();
            return process;
        }
    }
}
