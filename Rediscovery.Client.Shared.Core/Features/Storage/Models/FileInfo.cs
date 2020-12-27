using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rediscovery.Client.Shared.Core.Features.Storage.Models
{
    public class FileInfo<T>
    {
        public T Id { get; set; }
        public string Filename { get; set; }
        public string MimeType { get; set; }
        public DateTime CreateDate { get; set; }
        public Stream Stream { get; set; }
    }
}
