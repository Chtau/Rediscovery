using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels
{
    public class DiscoveryServiceInfo
    {
        private const string KeyIP = "ip";
        private const string KeyMeta = "meta";
        private const string KeyPort = "port";
        private const string KeyName = "name";
        private const string KeyDesktopName = "desktopname";
        private const string KeyDesktopOS = "desktopos";

        public string IPAddress { get; set; }

        public string Metadata { get; set; }

        public ushort Port { get; set; }

        public string Name { get; set; }

        public string DesktopName { get; set; }

        public string DesktopOS { get; set; }

        public override string ToString()
        {
            return $"{KeyIP}:{IPAddress};{KeyMeta}:{Metadata};{KeyPort}:{Port};{KeyName}:{Name};{KeyDesktopName}:{DesktopName};{KeyDesktopOS}:{DesktopOS}";
        }

        public void Parse(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var items = new List<string>();
                if (value.Contains(";"))
                {
                    var sp = value.Split(';');
                    if (sp != null && sp.Length > 0)
                        items.AddRange(sp);
                } else
                {
                    items.Add(value);
                }
                if (items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        if (item.Contains(":"))
                        {
                            var keyValue = item.Split(':');
                            var key = keyValue[0].ToLower();
                            switch (key)
                            {
                                case KeyIP:
                                    IPAddress = keyValue[1];
                                    break;
                                case KeyMeta:
                                    Metadata = keyValue[1];
                                    break;
                                case KeyPort:
                                    if (ushort.TryParse(keyValue[1], out ushort port))
                                        Port = port;
                                    break;
                                case KeyName:
                                    Name = keyValue[1];
                                    break;
                                case KeyDesktopName:
                                    DesktopName = keyValue[1];
                                    break;
                                case KeyDesktopOS:
                                    DesktopOS = keyValue[1];
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}
