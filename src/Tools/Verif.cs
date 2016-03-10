using System;
using System.Text.RegularExpressions;

namespace Tools
{
    /// <summary>
    /// This class contains static functions used to verify/check the pattern of a given value.
    /// </summary>
    public static class Verif
    {
        /// <summary>
        /// Used to check if the given string match an email pattern
        /// </summary>
        /// <param name="value">the value to be checked</param>
        /// <returns>True if valid; False is not valid</returns>
        public static bool Email(string value)
        {
            if (string.IsNullOrEmpty(value.Trim()))
                return false;
            /// This service used to run on a windows server 2003 with .net 3.5 and that's why I used a regex to double check that an email is correct.
            /// I realize that there are more elegant way to do this nowaday and this regex may not match future email patterns/tld. 
            return Regex.IsMatch(value, @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
        }

        /// <summary>
        /// Used to check if the given string match the given pattern
        /// </summary>
        /// <param name="value">the value to be checked</param>
        /// <param name="NumberStyle">the numeric pattern</param>
        /// <returns>True if valid; False is not valid</returns>
        public static bool Numeric(string value, System.Globalization.NumberStyles pattern)
        {
            Double result;
            return Double.TryParse(value, pattern, System.Globalization.CultureInfo.CurrentCulture, out result);
        }

    }
}
