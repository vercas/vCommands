using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Utilities
{
    /// <summary>
    /// Contains utilitary methods related to strings.
    /// </summary>
    public static class Strings
    {
        /// <summary>
        /// Enumerates through all the lines in the given string.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="includeLastEmptyLine">True to return an empty line at the end if the string ends in a newline, otherwise false.</param>
        /// <returns></returns>
        public static IEnumerable<string> GetLines(this string s, bool includeLastEmptyLine = false)
        {
            int startPos = -1;
            bool lastIsCr = false;

            for (int i = 0; i < s.Length; i += char.IsSurrogatePair(s, i) ? 2 : 1)
            {
                if (s[i] == '\r')
                    lastIsCr = true;
                else if (s[i] == '\n')
                {
                    if (startPos == -1)
                        yield return "";    //  Consecutive newlines (or first character being a newline) means an empty line.
                    else
                        yield return s.Substring(startPos, i - startPos - (lastIsCr ? 1 : 0));
                    //  If this is a newline after a carriage return (Windows-style newline), skip the carriage return.

                    startPos = -1;
                    lastIsCr = false;
                }
                else
                {
                    if (startPos == -1)
                        startPos = i;

                    lastIsCr = false;
                }
            }

            if (startPos != -1)
                yield return s.Substring(startPos, s.Length - startPos - (lastIsCr ? 1 : 0));
            else if (includeLastEmptyLine)
                yield return "";
        }
    }
}
