using DesktopFeatureConsole.Models;
using PluginFeature.Interfaces;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DesktopFeatureConsole
{
    public class Terminal
    {
        private readonly IPluginLogger _logger;

        public event EventHandler<CommandQueue<string, List<TerminalData>>> Output;

        private CommandQueue<string, List<TerminalData>> commandQueue;
        private Process process;

        public Terminal(IPluginLogger logger)
        {
            _logger = logger;
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
                process.ErrorDataReceived += Process_ErrorDataReceived;
                process.Start();
                process.BeginOutputReadLine();
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (commandQueue != null)
            {
                if (commandQueue.OutgoingData == null)
                    commandQueue.OutgoingData = new List<TerminalData>();
                commandQueue.OutgoingData.Add(new TerminalData { Line = e.Data, IsError = true });
                Output?.Invoke(this, commandQueue);
            }
            else
            {
                _logger?.LogCritical("Received process error data without having an active command queue");
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (commandQueue != null)
            {
                if (commandQueue.OutgoingData == null)
                    commandQueue.OutgoingData = new List<TerminalData>();
                commandQueue.OutgoingData.Add(new TerminalData { Line = e.Data });
                Output?.Invoke(this, commandQueue);
            } else
            {
                _logger?.LogCritical("Received process output data without having an active command queue");
            }
        }

        public void NewCommand(CommandQueue<string, List<TerminalData>> command)
        {
            if (commandQueue == null)
                commandQueue = command;
            else
            {
                if (command.DeviceId != commandQueue.DeviceId)
                {
                    throw new ArgumentException("Not allowed to send a additional command for a different deviceId", "command");
                }
                commandQueue.IncomingData = command.IncomingData;
            }
            InitTerminal();
            process.StandardInput.WriteLine(commandQueue.IncomingData + Environment.NewLine);
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
