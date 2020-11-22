using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Feature.Plugin.Models.ClientResources
{
    public class OpenWithIntent
    {
        public byte[] Content { get; set; }
        public string Uri { get; set; }
        public string TextContent { get; set; }
        public string HtmlContent { get; set; }
        public string Mime { get; set; }
        public string Title { get; set; }
    }
}
