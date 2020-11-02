using System;

namespace _010.正则表达式匹配
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = "aa";
            var p = "a";
            var solution = new Solution();
            Console.WriteLine(solution.IsMatch(s, p));
            s = "aa";
            p = "a*";
            Console.WriteLine(solution.IsMatch(s, p));
            s = "ab";
            p = ".*";
            Console.WriteLine(solution.IsMatch(s, p));
            s = "aab";
            p = "c*a*b";
            Console.WriteLine(solution.IsMatch(s, p));
            s = "mississippi";
            p = "mis*is*p*.";
            Console.WriteLine(solution.IsMatch(s, p));
        }
    }

    public class Solution
    {
        public bool IsMatch(string s, string p)
        {
            if (string.IsNullOrEmpty(p))
            {
                return string.IsNullOrEmpty(s);
            }

            bool first_match = (!string.IsNullOrEmpty(s) &&
                                (p.ToCharArray()[0] == s.ToCharArray()[0] || p.ToCharArray()[0] == '.'));
            if (p.Length >= 2 && p.ToCharArray()[1] == '*')
            {
                return (IsMatch(s, p.Substring(2)) || (first_match && IsMatch(s.Substring(1), p)));
            }
            else
            {
                return first_match && IsMatch(s.Substring(1), p.Substring(1));
            }
        }
    }
}