using System.Text.RegularExpressions;

namespace Temiang.Avicenna
{
    public static class StringExtensions
    {
        public static string ReplaceWholeWord(this string input, string word, string replacement)
        {
            // Escape karakter khusus regex dari 'word' supaya aman
            string pattern = Regex.Escape(word) + @"(?![a-zA-Z0-9])";
            return Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase);
        }
    }
}