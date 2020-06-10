using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Core
{
    public class EntityContent<I, T>
    {
        public I Id { get; set; }
        public T Content { get; set; }

        public EntityContent(I id, T content)
        {
            Id = id;
            Content = content;
        }
    }
}
