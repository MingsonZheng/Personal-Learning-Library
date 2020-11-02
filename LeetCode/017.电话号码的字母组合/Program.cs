using System;
using System.Collections.Generic;

namespace _017.电话号码的字母组合
{
    class Program
    {
        static void Main(string[] args)
        {
            var solution = new Solution();
            Console.WriteLine(string.Join(",", solution.LetterCombinations("23")));
        }
    }

    public class Solution
    {
        Dictionary<string, string> dic = new Dictionary<string, string>()
        {
            {"2", "abc"},
            {"3", "def"},
            {"4", "ghi"},
            {"5", "jkl"},
            {"6", "mno"},
            {"7", "pqrs"},
            {"8", "tuv"},
            {"9", "wxyz"}
        };

        IList<string> result = new List<string>();

        public IList<string> LetterCombinations(string digits)
        {
            if (digits.Length > 0) BackTrack("", digits);

            return result;
        }

        public void BackTrack(string combinations, string nextDigits)
        {
            if (nextDigits.Length == 0) result.Add(combinations);
            else
            {
                var digits = dic[nextDigits.Substring(0, 1)];
                for (int i = 0; i < digits.Length; i++)
                {
                    var combination = digits.Substring(i, 1);
                    BackTrack(combinations + combination, nextDigits.Remove(0, 1));
                }
            }
        }
    }
}