using System;
using System.Collections.Generic;
using System.Text;

namespace _006.Z字形变换
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = "LEETCODEISHIRING";
            var solution = new Solution();
            Console.WriteLine(solution.Convert(s, 3));
            s = "LEETCODEISHIRING";
            Console.WriteLine(solution.Convert(s, 4));
        }
    }

    public class Solution
    {
        public string Convert(string s, int numRows)
        {
            if (numRows == 1)
            {
                return s;
            }

            var rows = new List<StringBuilder>();
            for (int i = 0; i < Math.Min(numRows, s.Length); i++)
            {
                rows.Add(new StringBuilder());
            }

            int curRow = 0;
            bool goingDown = false;

            foreach (var c in s.ToCharArray())
            {
                rows[curRow].Append(c);
                if (curRow == 0 || curRow == numRows - 1)
                {
                    goingDown = !goingDown;
                }

                curRow += goingDown ? 1 : -1;
            }

            var ret = new StringBuilder();
            foreach (var row in rows)
            {
                ret.Append(row);
            }

            return ret.ToString();
        }
    }
}