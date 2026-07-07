// task01/StringExtensions.cs
using System.Linq;

namespace task01;

public static class StringExtensions
{
    public static bool IsPalindrome(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        var cleanString = new string(input
            .ToLower()
            .Where(c => !char.IsPunctuation(c) && !char.IsWhiteSpace(c))
            .ToArray());

        if (string.IsNullOrEmpty(cleanString))
        {
            return false;
        }

        var reversedString = new string(cleanString.Reverse().ToArray());

        return cleanString == reversedString;
    }
}