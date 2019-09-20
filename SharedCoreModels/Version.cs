using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels
{
    public class Version
    {
        public int Major { get; set; }

        public int Minor { get; set; }

        public int Patch { get; set; }

        public string Label { get; set; }

        public override string ToString()
        {
            return ConvertFrom(this);
        }

        public static string ConvertFrom(Version version)
        {
            string v = version.Major + "." + version.Minor + "." + version.Patch;
            if (!string.IsNullOrWhiteSpace(version.Label))
                v += "-" + version.Label;
            return v;
        }

        public static Version ConvertTo(string version)
        {
            var v = new Version
            {
                Major = 0,
                Minor = 0,
                Patch = 0,
                Label = "",
            };
            if (!string.IsNullOrWhiteSpace(version))
            {
                var v1 = version.Split('.');
                if (v1.Length > 0)
                {
                    if (int.TryParse(v1[0], out int maj))
                        v.Major = maj;
                }
                if (v1.Length > 1)
                {
                    if (int.TryParse(v1[1], out int min))
                        v.Minor = min;
                }
                if (v1.Length == 3)
                {
                    if (v1[2].Contains("-"))
                    {
                        var v2 = v1[2].Split('-');
                        if (v2.Length > 0)
                        {
                            if (int.TryParse(v2[0], out int pat))
                                v.Patch = pat;
                        }
                        if (v2.Length > 1)
                        {
                            v.Label = v2[1];
                        }
                    }
                    else
                    {
                        if (int.TryParse(v1[2], out int pat))
                            v.Patch = pat;
                    }
                }
            }
            return v;
        }
    }
}
