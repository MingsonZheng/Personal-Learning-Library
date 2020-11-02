using System;
using System.Collections.Generic;

namespace _003.无重复字符的最长子串
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = "abcabcbb";
            var solution = new Solution();
            Console.WriteLine(solution.LengthOfLongestSubstring4(s));
            s = "bbbbb";
            Console.WriteLine(solution.LengthOfLongestSubstring4(s));
            s = "pwwkew";
            Console.WriteLine(solution.LengthOfLongestSubstring4(s));
        }
    }

    public class Solution
    {
        #region 方法一：暴力法

        public int LengthOfLongestSubstring(string s)
        {
            int n = s.Length;
            int ans = 0;
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j <= n; j++)
                    if (allUnique(s, i, j)) ans = Math.Max(ans, j - i);
            return ans;
        }

        public bool allUnique(string s, int start, int end)
        {
            var set = new HashSet<char>();
            for (int i = start; i < end; i++)
            {
                var ch = s.ToCharArray()[i];
                if (set.Contains(ch)) return false;
                set.Add(ch);
            }

            return true;
        }

        #endregion

        #region 方法二：滑动窗口

        public int LengthOfLongestSubstring2(string s)
        {
            int n = s.Length;
            var set = new HashSet<char>();
            int ans = 0, i = 0, j = 0;
            while (i < n && j < n)
            {
                // try to extend the range [i, j]
                if (!set.Contains((s.ToCharArray()[j])))
                {
                    set.Add(s.ToCharArray()[j++]);
                    ans = Math.Max(ans, j - i);
                }
                else
                    set.Remove(s.ToCharArray()[i++]);
            }

            return ans;
        }

        #endregion

        #region 方法三：优化的滑动窗口

        public int LengthOfLongestSubstring3(string s)
        {
            int n = s.Length;
            int ans = 0;
            var dic = new Dictionary<char, int>();// current index of character
            // try to extend the range [i, j]
            for (int j = 0, i = 0; j < n; j++)
            {
                if (dic.ContainsKey(s.ToCharArray()[j]))
                {
                    i = Math.Max(dic[s.ToCharArray()[j]], i);
                    dic[s.ToCharArray()[j]] = j + 1;
                }
                else
                    dic.Add(s.ToCharArray()[j], j + 1);
                ans = Math.Max(ans, j - i + 1);
            }
            return ans;
        }

        #endregion

        #region 非ETL解法

        public int LengthOfLongestSubstring4(string s)
        {
            List<char> ls = new List<char>();

            int n = s.Length;

            int intMaxLength = 0;

            for (int i = 0; i < n; i++)
            {
                if (ls.Contains(s[i]))
                {
                    ls.RemoveRange(0, ls.IndexOf(s[i]) + 1);
                }

                ls.Add(s[i]);
                intMaxLength = ls.Count > intMaxLength ? ls.Count : intMaxLength;
            }

            return intMaxLength;
        }

        #endregion
    }
}
