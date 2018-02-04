using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Venter.Utilities
{
    public static class StringExt
    {
        /// <summary>
        /// Generates a permalink slug for passed string
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns>clean slug string (ex. "some-cool-topic")</returns>
        public static string GenerateSlug(this string phrase)
        {
            string s = phrase.Normalize().ToLower();

            s = Regex.Replace(s, @"[^a-z0-9\s-]", "");                      // remove invalid characters
            s = Regex.Replace(s, @"\s+", " ").Trim();                       // single space
            s = s.Substring(0, s.Length <= 1900 ? s.Length : 1900).Trim();      // cut and trim
            s = Regex.Replace(s, @"\s", "-");                               // insert hyphens

            return s;
        }

        public static string ToLine(this string[] paragraphs, bool newline = true)
        {
            string ret = string.Empty;
            bool first = true;

            foreach (var item in paragraphs)
            {
                if (newline)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        ret += Environment.NewLine;
                    }
                }

                ret += item;
            }

            return ret;
        }
    }
}
