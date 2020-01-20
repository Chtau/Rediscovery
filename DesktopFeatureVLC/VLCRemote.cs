using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Net.Sockets;
using System.Text;

namespace DesktopFeatureVLC
{
    public class VLCRemote
    {
        public enum VlcCommand
        {
            Add,
            Enqueue,
            Play,
            F,
            Is_Playing,
            Get_Time,
            Seek,
            Pause,
            FastForward,
            Rewind
        }

        // maximum 2 second wait on results. 
        const int WaitTimeout = 2000;

        static ASCIIEncoding ASCIIEncoding = new ASCIIEncoding();

        Process vlcProcess;
        TcpClient client;

        static int GetParentProcessId(int Id)
        {
            int parentPid = 0;
            using (ManagementObject mo = new ManagementObject("win32_process.handle='"
            + Id.ToString() + "'"))
            {
                mo.Get();
                parentPid = Convert.ToInt32(mo["ParentProcessId"]);
            }
            return parentPid;
        }


        public VLCRemote()
        {
            string vlcPath = @"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe";

            if (vlcPath == null)
            {
                throw new ApplicationException("Can not find the VLC executable!");
            }

            var info = new ProcessStartInfo(vlcPath, "-I rc --rc-host=localhost:9876");
            vlcProcess = Process.Start(info);
            client = new TcpClient("localhost", 9876);
        }

        public Process VlcPlaybackProcess
        {
            get
            {
                var currentProcessId = Process.GetCurrentProcess().Id;
                Process vlcProcess = null;
                foreach (var process in Process.GetProcessesByName("vlc"))
                {
                    if (GetParentProcessId(process.Id) == currentProcessId)
                    {
                        vlcProcess = process;
                        break;
                    }
                }
                return vlcProcess;
            }
        }

        public void Add(string filename)
        {
            SendCommand(VlcCommand.Add, filename);
        }

        public void Enqueue(string filename)
        {
            SendCommand(VlcCommand.Enqueue, filename);
        }

        public void Play()
        {
            SendCommand(VlcCommand.Play);
        }

        public void Pause()
        {
            SendCommand(VlcCommand.Pause);
        }

        public void GoToFullScreen()
        {
            SendCommand(VlcCommand.F, "on");
        }

        public bool IsPlaying
        {
            get
            {
                SendCommand(VlcCommand.Is_Playing);
                string result = WaitForResult().Trim();
                return result == "1";
            }
        }

        public int Position
        {
            get
            {
                SendCommand(VlcCommand.Get_Time);
                var result = WaitForResult().Trim();
                return Convert.ToInt32(result);
            }
            set
            {
                SendCommand(VlcCommand.Seek, value.ToString());
            }
        }

        public void FastForward()
        {
            SendCommand(VlcCommand.FastForward);
        }

        public void Rewind()
        {
            SendCommand(VlcCommand.Rewind);
        }

        string WaitForResult()
        {
            string result = "";
            DateTime start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds < WaitTimeout)
            {
                result = ReadTillEnd();
                if (!string.IsNullOrEmpty(result))
                {
                    break;
                }
            }
            return result;
        }

        void SendCommand(VlcCommand command)
        {
            SendCommand(command, null);
        }

        void SendCommand(VlcCommand command, string param)
        {
            // flush old stuff
            ReadTillEnd();

            string packet = Enum.GetName(typeof(VlcCommand), command).ToLower();
            if (param != null)
            {
                packet += " " + param;
            }
            packet += Environment.NewLine;

            var buffer = ASCIIEncoding.GetBytes(packet);
            client.GetStream().Write(buffer, 0, buffer.Length);
            client.GetStream().Flush();


            Trace.Write(packet);

        }

        public string ReadTillEnd()
        {
            StringBuilder sb = new StringBuilder();
            while (client.GetStream().DataAvailable)
            {
                int b = client.GetStream().ReadByte();
                if (b >= 0)
                {
                    sb.Append((char)b);
                }
                else
                {
                    break;
                }
            }
            return sb.ToString();
        }

    }
}
