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
            string v = Major + "." + Minor + "." + Patch;
            if (!string.IsNullOrWhiteSpace(Label))
                v += "-" + Label;
            return v;
        }
    }
}
