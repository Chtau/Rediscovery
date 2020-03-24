using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Features.DesktopFeatures
{
    public interface IHtmlUIService
    {
        string GetIndexFile(string directory);
        List<(HtmlUIService.DefaultFileType type, string fileName, string fileContent)> GetDefaultFiles();
        string NoUIHtmlDefault();
    }
}
