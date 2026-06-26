using System;
using System.Text;

namespace task01
{
    public static class StringExtensions
    {
        public static bool IsPalindrome(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            StringBuilder sb = new StringBuilder();

            foreach (char ch in input)
            {
                if (!char.IsPunctuation(ch) && !char.IsWhiteSpace(ch))
                {
                    sb.Append(char.ToLowerInvariant(ch));
                }
            }

            string cleaned = sb.ToString();

            if (cleaned.Length == 0)
            {
                return false;
            }

            int left = 0;
            int right = cleaned.Length - 1;

            while (left < right)
            {
                if (cleaned[left] != cleaned[right])
                {
                    return false;
                }
                left++;
                right--;
            }

            return true;
        }
    }
}