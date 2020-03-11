using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.DesktopFeatures.FeatureUIService))]
namespace Rediscovery.Features.DesktopFeatures
{
    public class FeatureUIService : IFeatureUIService
    {
        private Features.Connection.IConnect connect => DependencyService.Get<Features.Connection.IConnect>() ?? new Features.Connection.Connect();
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private Services.IFileSystem fileSystem => DependencyService.Get<Services.IFileSystem>() ?? new Services.FileSystem();

        public void SaveUI(Guid featureId, Action<bool, string> callback)
        {
            Task.Run(async () =>
            {
                string directory = OnArchiveDirectory(featureId);
                try
                {
                    if (System.IO.Directory.Exists(directory))
                    {
                        System.IO.Directory.Delete(directory);
                    }
                } catch (Exception ex)
                {
                    logger.Error(ex);
                }
                try
                {
                    var archive = await connect.GetUIArchive(featureId);
                    if (archive != null)
                    {
                        archive.ExtractToDirectory(directory);
                        callback?.Invoke(true, directory);
                    }
                    else
                    {
                        logger.Message($"No UI Archive received for Feature Id:{featureId}");
                        callback?.Invoke(false, directory);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                } finally
                {
                    callback?.Invoke(false, directory);
                }
            });
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
