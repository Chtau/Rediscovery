using System;
using System.Collections.Generic;
using System.Text;

public static class StringExtensions
{
    public static string EmptyIfNull(this string value)
    {
        if (value == null)
            value = string.Empty;
        return value;
    }
}
