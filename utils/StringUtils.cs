
namespace Bench
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public static partial class StringUtils
    {
        /// <summary>
        /// string compare helper function
        /// </summary>
        /// <param name="x">first string</param>
        /// <param name="y">second string</param>
        /// <param name="ignoreCase">ignore case</param>
        /// <returns>true if x equal to y</returns>
        public static bool Match(this string x, string y, bool ignoreCase = true)
        {
            return string.Compare(x, y, ignoreCase) == 0;
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="text">TBD</param>
        /// <param name="pattern">TBD</param>
        /// <param name="caseSensitive">TBD</param>
        /// <returns>TBD</returns>
        public static bool Like(this string text,string pattern, bool ignoreCase = false)
        {
            pattern = pattern.Replace(".", @"\.");
            pattern = pattern.Replace("?", ".");
            pattern = pattern.Replace("*", ".*?");
            pattern = pattern.Replace(@"\", @"\\");
            pattern = pattern.Replace(" ", @"\s");
            Regex regex = new Regex(pattern, ignoreCase ? RegexOptions.None : RegexOptions.IgnoreCase);
            return regex.IsMatch(text);
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

        /// <summary>
        /// Return a safe sub string
        /// </summary>
        /// <param name="str">string value</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">substring length</param>
        /// <returns></returns>
        public static string SafeSubString(this string str, int startIndex, int length)
        {
            if (str == null || str.Length < startIndex + length)
            {
                return str;
            }

            return str.Substring(startIndex, length);
        }
    }

}
