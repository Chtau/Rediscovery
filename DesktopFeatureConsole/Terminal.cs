using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DesktopFeatureConsole
{
    public class Terminal
    {
        public event EventHandler<string> Output;

        private Process process;

        public Terminal()
        {
            
        }

        private void InitTerminal()
        {
            if (process == null)
            {
                process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.OutputDataReceived += Process_OutputDataReceived;
                process.Start();
                process.BeginOutputReadLine();
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Output?.Invoke(this, e.Data);
        }

        public void WriteLine(string input)
        {
            InitTerminal();
            process.StandardInput.WriteLine(input + Environment.NewLine);
        }

        public void Close()
        {
            if (process != null)
            {
                process.Kill();
            }
        }
    }
}
