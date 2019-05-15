using System;
using System.Collections.Generic;
using System.Text;

namespace ReflectionTester
{
    internal static class ObjectExtension
    {
        internal static object Wrap(this string s)
        {
            return new
            {
                EncapsulatedString = s
            };
        }

        internal static object Unwrap(this object o)
        {
            var property = o.GetType().GetProperty("EncapsulatedString");
            return property != null
                ? property.GetValue(o, null)
                : o;
        }
    }
}
