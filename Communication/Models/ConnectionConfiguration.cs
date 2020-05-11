using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceConsumer.Models
{
    public class ConnectionConfiguration
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Address { get; set; }
        public string Token { get; set; }
        public ConnectionState State { get; set; }
    }
}
