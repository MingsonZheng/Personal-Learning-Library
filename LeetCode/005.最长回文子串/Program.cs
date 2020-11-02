using System;

namespace _005.最长回文子串
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = "babad";
            var solution = new Solution();
            Console.WriteLine(solution.LongestPalindrome(s));
            s = "cbbd";
            Console.WriteLine(solution.LongestPalindrome(s));
        }
    }

    public class Solution
    {
        public string LongestPalindrome(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return "";
            }

            int start = 0, end = 0;
            for (int i = 0; i < s.Length; i++)
            {
                int len1 = expandAroundCenter(s, i, i);
                int len2 = expandAroundCenter(s, i, i + 1);
                int len = Math.Max(len1, len2);
                if (len > end - start)
                {
                    start = i - (len - 1) / 2;
                    end = i + len / 2;
                }
            }

            return s.Substring(start, end - start + 1);
        }

        private int expandAroundCenter(string s, int left, int right)
        {
            int L = left, R = right;
            while (L >= 0 && R < s.Length && s.ToCharArray()[L] == s.ToCharArray()[R])
            {
                L--;
                R++;
            }

            return R - L - 1;
        }
    }
}