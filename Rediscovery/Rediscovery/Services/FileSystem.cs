using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Services.FileSystem))]
namespace Rediscovery.Services
{
    public class FileSystem : BaseService, IFileSystem
    {
        public string FeatureHtmlUIDirectory()
        {
            var dir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "ui");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
