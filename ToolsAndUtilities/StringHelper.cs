using System;
using System.Linq;

namespace ToolsAndUtilities
{
    public static class StringHelper
    {
        public static bool IsAnagram(this string baseString, string anagramString)
        {
            var charSet1 = baseString.ToArray().OrderBy(a => a);
            var charSet2 = anagramString.ToArray().OrderBy(a => a);
            return charSet1.SequenceEqual(charSet2);
        }
    }
}