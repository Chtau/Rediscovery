using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.DesktopFeatures.FeatureUIService))]
namespace Rediscovery.Features.DesktopFeatures
{
    public class FeatureUIService : IFeatureUIService
    {
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private Services.IFileSystem fileSystem => DependencyService.Get<Services.IFileSystem>() ?? new Services.FileSystem();

        public void SaveUI(ZipArchive zipArchive, Guid featureId)
        {
            zipArchive.ExtractToDirectory(OnArchiveDirectory(featureId));
        }

        public string UIDirectory(Guid featureId)
        {
            return OnArchiveDirectory(featureId);
        }

        private string OnArchiveDirectory(Guid featureId)
        {
            string dir = System.IO.Path.Combine(fileSystem.FeatureHtmlUIDirectory(), featureId.ToNormalizedString());
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
