using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace DesktopFeatureVLC
{
    public class VLC
    {
        public static string GetApplicationFolder()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        /*[DllImport("libvlc", CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_media_player_set_pause")]
        internal static extern void LibVLCMediaPlayerSetPause(IntPtr mediaPlayer, bool pause);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetDllDirectory(string lpPathName);*/

        public void Start()
        {
            /*using (var libVLC = new LibVLC())
            {
                var media = new Media(libVLC, "https://www.youtube.com/watch?v=dQw4w9WgXcQ", FromType.FromLocation);
                media.Parse(MediaParseOptions.ParseNetwork).GetAwaiter();
                using (var mp = new MediaPlayer(media.SubItems.First()))
                {
                    var r = mp.Play();
                    Console.ReadKey();
                }
            }*/

            /*SetDllDirectory(@"C:\Program Files (x86)\VideoLAN\VLC");

            var processes = Process.GetProcesses();
            Process myProcess = processes.FirstOrDefault(p => p.ProcessName == "vlc");
            if (myProcess != null)
                LibVLCMediaPlayerSetPause(myProcess.Handle, true);

            return;*/
            var libDirectory = new DirectoryInfo(Path.Combine(GetApplicationFolder(), "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

            var options = new string[]
            {
                // VLC options can be given here. Please refer to the VLC command line documentation.
            };

            var mediaPlayer = new Vlc.DotNet.Core.VlcMediaPlayer(libDirectory);

            var mediaOptions = new string[]
            {
                ":sout=#file{dst="+Path.Combine(Environment.CurrentDirectory, "output.mov")+"}",
                ":sout-keep"
            };

            //mediaPlayer.SetMedia(new Uri("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_h264.mov"), mediaOptions);
            mediaPlayer.SetMedia(new Uri(@"C:\DEV\Code\Workspaces\Repos\Rediscovery\DesktopFeatureTestApp\bin\x64\Debug\netcoreapp3.1\Ashley O – On a Roll _ Official Music Video (2019-06-13).mp4"));
            mediaPlayer.Play();
            bool playFinished = false;
            mediaPlayer.PositionChanged += (sender, e) =>
            {
                Console.Write("\r" + Math.Floor(e.NewPosition * 100) + "%");
            };

            mediaPlayer.EncounteredError += (sender, e) =>
            {
                Console.Error.Write("An error occurred");
                playFinished = true;
            };

            mediaPlayer.EndReached += (sender, e) => {
                playFinished = true;
            };

            mediaPlayer.Play();

            // Ugly, sorry, that's just an example...
            while (!playFinished)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
            }
        }
    }
}
