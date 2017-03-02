
namespace Bench
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;

    public static partial class StringUtils
    {
        /// <summary>
        /// string compare helper function
        /// </summary>
        /// <param name="x">first string</param>
        /// <param name="y">second string</param>
        /// <param name="ignoreCase">ignore case</param>
        /// <returns>true if x equal to y</returns>
        public static bool Matches(this string x, string y, bool ignoreCase = true)
        {
            return string.Compare(x, y, ignoreCase) == 0;
        }

        /// <summary>
        /// Return a safe string
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns>if str is null, return empty. other the string value</returns>
        public static string SafeString(this string str)
        {
            return str ?? string.Empty;
        }
    }

}
