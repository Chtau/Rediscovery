using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Desktop.Hubs
{
    public class Manifest : Hub
    {
        public void Hello(string identifier)
        {
            var minfestContent = new Dictionary<string, string>();
            minfestContent.Add("identitfier", "1");// TODO: should be a unique static identifier
            minfestContent.Add("name", "CT");

            Clients.Caller.SendAsync("Hello:" + DateTime.Now, minfestContent);
        }
    }
}
