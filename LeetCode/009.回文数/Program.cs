using System;

namespace _009.回文数
{
    class Program
    {
        static void Main(string[] args)
        {
            int x = 121;
            var solution = new Solution();
            Console.WriteLine(solution.IsPalindrome(x));
            x = -121;
            Console.WriteLine(solution.IsPalindrome(x));
            x = 10;
            Console.WriteLine(solution.IsPalindrome(x));
        }
    }

    public class Solution
    {
        public bool IsPalindrome(int x)
        {
            if (x < 0 || x % 10 == 0 && x != 0)
            {
                return false;
            }

            int revertedNumber = 0;
            while (x > revertedNumber)
            {
                revertedNumber = revertedNumber * 10 + x % 10;
                x /= 10;
            }

            return x == revertedNumber || x == revertedNumber / 10;
        }
    }
}