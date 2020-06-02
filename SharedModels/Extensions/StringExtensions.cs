using System;
using System.Collections.Generic;
using System.Text;

public static class StringExtensions
{
    public static Guid SafeGuid(this string str)
    {
        if (!string.IsNullOrWhiteSpace(str))
        {
            if (Guid.TryParse(str, out Guid guid))
                return guid;
            try
            {
                return new Guid(str);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Fail($"Could not parse string to Guid. Value:{str} Methode:{nameof(SafeGuid)} Exception:{ex.ToString()}");
            }
        }
        return Guid.Empty;
    }
}
