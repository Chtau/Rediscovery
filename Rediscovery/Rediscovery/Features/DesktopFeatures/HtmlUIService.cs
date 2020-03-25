using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.DesktopFeatures.HtmlUIService))]
namespace Rediscovery.Features.DesktopFeatures
{
    public class HtmlUIService : BaseService, IHtmlUIService
    {
        private IResourceProvider resourceProvider => DependencyService.Get<IResourceProvider>() ?? new ResourceProvider();

        public string GetIndexFile(string directory)
        {
            if (!string.IsNullOrWhiteSpace(directory) && System.IO.Directory.Exists(directory))
            {
                // find start file
                string startFile = "";
                if (System.IO.File.Exists(System.IO.Path.Combine(directory, "Index.html")))
                    startFile = System.IO.Path.Combine(directory, "Index.html");
                else if (System.IO.File.Exists(System.IO.Path.Combine(directory, "index.html")))
                    startFile = System.IO.Path.Combine(directory, "index.html");
                else if (System.IO.File.Exists(System.IO.Path.Combine(directory, "default.html")))
                    startFile = System.IO.Path.Combine(directory, "default.html");
                else if (System.IO.File.Exists(System.IO.Path.Combine(directory, "Default.html")))
                    startFile = System.IO.Path.Combine(directory, "Default.html");
                return startFile;
            }
            return null;
        }

        public enum DefaultFileType
        {
            JS,
            LINK,
            HTML
        }

        public List<(DefaultFileType type, string fileName, string fileContent)> GetDefaultFiles()
        {
            var ret = new List<(DefaultFileType type, string fileName, string fileContent)>();
            ret.Add((DefaultFileType.JS, "exchange.js", resourceProvider.ReadResource("exchange.js")));
            return ret;
        }

        public string NoUIHtmlDefault()
        {
            return resourceProvider.ReadResource("index.html");
        }
    }
}
