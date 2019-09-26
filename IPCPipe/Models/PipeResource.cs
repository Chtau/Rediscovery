using System;
using System.Collections.Generic;
using System.Text;

namespace IPCPipe.Models
{
    public class PipeResource<T> : IPipeResource
    {
        public string ResourceName { get; set; }
        public T Entity { get; set; }
    }
}
