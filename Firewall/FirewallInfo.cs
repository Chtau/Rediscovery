using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Firewall
{
    public class FirewallInfo
    {
        private const string FirewallBaseCommand = "netsh advfirewall firewall ";
        private const string FirewallShowRuleCommand = "show rule name=\"{0}\"";

        public FirewallInfo()
        {
            
        }


        public bool RuleExists(string ruleName)
        {
            bool result = false;
            bool hasResult = false;
            Task.Run(() =>
            {
                var prc = GetProcess((object sender, DataReceivedEventArgs e) =>
                {
                    System.Diagnostics.Debug.Print(e.Data);

                    hasResult = true;
                });
                string cmd = FirewallBaseCommand + string.Format(FirewallShowRuleCommand, ruleName) + Environment.NewLine;
                prc.StandardInput.WriteLine(cmd);
            });

            do
            {
                System.Threading.Thread.Sleep(10);
            } while (!hasResult);
            return result;
        }

        private Process GetProcess(DataReceivedEventHandler eventHandlerConsoleOutput)
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
