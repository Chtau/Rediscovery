using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mime;
using System.Text;

namespace Rediscovery.Features.ClientFeatures.Models
{
    public class IntentReceivedModel
    {
        public byte[] Content { get; }
        public string Uri { get; }
        public string TextContent { get; }
        public string HtmlContent { get; }
        public string Mime { get; }
        public string Title { get; }

        public IntentReceivedModel(byte[] content, string uri, string textContent, string htmlContent, string mime, string title)
        {
            Content = content;
            Uri = uri;
            TextContent = textContent;
            HtmlContent = htmlContent;
            Mime = mime;
            Title = title;
        }
    }
}
