using System;

namespace _007.整数反转
{
    class Program
    {
        static void Main(string[] args)
        {
            int x = 123;
            var solution = new Solution();
            Console.WriteLine(solution.Reverse(x));
            x = -123;
            Console.WriteLine(solution.Reverse(x));
            x = 120;
            Console.WriteLine(solution.Reverse(x));
        }
    }

    public class Solution
    {
        public int Reverse(int x)
        {
            int rev = 0;
            while (x != 0)
            {
                int pop = x % 10;
                x /= 10;

                if (rev > Int32.MaxValue / 10 || (rev == Int32.MaxValue / 10 && pop > 7))
                {
                    return 0;
                }

                if (rev < Int32.MinValue / 10 || (rev == Int32.MinValue / 10 && pop < -8))
                {
                    return 0;
                }

                rev = rev * 10 + pop;
            }

            return rev;
        }
    }
}