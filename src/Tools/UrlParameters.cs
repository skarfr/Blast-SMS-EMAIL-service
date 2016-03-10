using System;
using System.Collections.Generic;
using System.Web;

// None        HttpUtility.UrlEncode       Uri.EscapeDataString       Uri.EscapeUriString
// &                   %26                         %26                         &
// $                   %24                         %24                         $
// +                   %2b                         %2B                         +
// Space               +                           %20                         %20
// %                   %25                         %25                         %25
// <                   %3c                         %3C                         %3C
// http://blogs.msdn.com/b/yangxind/archive/2006/11/09/don-t-use-net-system-uri-unescapedatastring-in-url-decoding.aspx

namespace Tools
{
    /// <summary>
    /// This class contains static functions related to Url Parameters
    /// </summary>
    public static class UrlParameters
    {
        /// <summary>
        /// This staic list contains all the different functions of Url Encoding available in Blast
        /// Please refer to the .net doc (v3.5 at this time) or to this class header comment to see differences between each encoding functions
        /// </summary>
        public static List<string> UrlEncodeList = new List<string> { "HttpUtility.UrlEncode", "Uri.EscapeDataString", "Uri.EscapeUriString", "None" };

        /// <summary>
        /// Used to encode the given value with the given urlEncoding functions
        /// </summary>
        /// <param name="value">the value to be encoded. Default: ""</param>
        /// <param name="urlEncoding">the urlEncoding to be applied. Default: "None"</param>
        /// <returns>the encoded value</returns>
        public static string Encode(string value="", string urlEncoding="None")
        {
            if (!UrlEncodeList.Contains(urlEncoding))
                throw new Exception("Tools::UrlEncoding: The encoding: '" + urlEncoding + "' is not available in this version of Blast.");

            if(string.IsNullOrEmpty(value))
                return value;

            switch (urlEncoding) {
                case "HttpUtility.UrlEncode":
                    return HttpUtility.UrlEncode(value);
        
                case "Uri.EscapeDataString":
                    return Uri.EscapeDataString(value);

                case "Uri.EscapeUriString":
                    return Uri.EscapeUriString(value);

                case "None":
                default:
                    return value;
            }
        }
    }
}



