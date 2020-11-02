using System;
using System.Collections.Generic;

namespace _013.罗马数字转整数
{
    class Program
    {
        static void Main(string[] args)
        {
            var solution = new Solution();
            Console.WriteLine(solution.RomanToInt("III"));
            Console.WriteLine(solution.RomanToInt("IV"));
            Console.WriteLine(solution.RomanToInt("IX"));
            Console.WriteLine(solution.RomanToInt("LVIII"));
            Console.WriteLine(solution.RomanToInt("MCMXCIV"));
        }
    }

    public class Solution
    {
        public int RomanToInt(string s)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>()
            {
                {"I",1},{"IV",4},{"V",5},{"IX",9},{"X",10},{"XL",40},{"L",50},{"XC",90},{"C",100},{"CD",400},{"D",500},{"CM",900},{"M",1000}
            };
            int result = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (i + 1 < s.Length && dic.ContainsKey(s.Substring(i, 2)))
                {
                    result += dic[s.Substring(i, 2)];
                    i++;
                }
                else
                {
                    result += dic[s.Substring(i, 1)];
                }
            }
            return result;
        }
    }
}