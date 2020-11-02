using System;

namespace _014.最长公共前缀
{
    class Program
    {
        static void Main(string[] args)
        {
            Solution solution = new Solution();
            string[] strs = new string[]
            {
                "flower",
                "flow",
                "flight"
            };
            Console.WriteLine(solution.LongestCommonPrefix(strs));
            strs = new string[]
            {
                "dog",
                "racecar",
                "car"
            };
            Console.WriteLine(solution.LongestCommonPrefix(strs));
        }
    }

    public class Solution
    {
        public string LongestCommonPrefix(string[] strs)
        {
            if (strs != null && strs.Length == 0) return "";
            for (int i = 0; i < strs[0].Length; i++)
            {
                var str = strs[0].Substring(i, 1);
                for (int j = 1; j < strs.Length; j++)
                {
                    if (i == strs[j].Length || str != strs[j].Substring(i, 1))
                        return strs[0].Substring(0, i);
                }
            }

            return strs[0];
        }
    }
}