using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationBase.Models
{
    public class ExchangeEntity<T>
    {
        public string Sid { get; set; }
        public T Entity { get; set; }
    }
}
