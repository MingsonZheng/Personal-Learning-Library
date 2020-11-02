using System;
using System.Collections.Generic;
using System.Text;

namespace _022._括号生成
{
    class Program
    {
        static void Main(string[] args)
        {
            var solution = new Solution();
            var res = solution.GenerateParenthesis(3);
            foreach (var re in res)
            {
                Console.WriteLine(re);
            }
        }
    }

    public class Solution
    {
        public IList<string> GenerateParenthesis(int n)
        {
            var res = new List<string>();
            var cur = new StringBuilder();
            Backtrack(res, cur, 0, 0, n);
            return res;
        }

        public void Backtrack(IList<string> res, StringBuilder cur, int left, int right, int max)
        {
            if (cur.Length == max * 2)
            {
                res.Add(cur.ToString());
            }
            else
            {
                if (left < max)
                {
                    cur.Append("(");
                    Backtrack(res, cur, left + 1, right, max);
                    cur.Remove(cur.Length - 1, 1);
                }

                if (right < left)
                {
                    cur.Append(")");
                    Backtrack(res, cur, left, right + 1, max);
                    cur.Remove(cur.Length - 1, 1);
                }
            }

        }
    }
}