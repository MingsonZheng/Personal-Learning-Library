using System;
using System.Text;

namespace _012.整数转罗马数字
{
    class Program
    {
        static void Main(string[] args)
        {
            int num = 3;
            var solution = new Solution();
            Console.WriteLine(solution.IntToRoman(num));
            num = 4;
            Console.WriteLine(solution.IntToRoman(num));
            num = 9;
            Console.WriteLine(solution.IntToRoman(num));
            num = 58;
            Console.WriteLine(solution.IntToRoman(num));
            num = 1994;
            Console.WriteLine(solution.IntToRoman(num));
        }
    }

    public class Solution
    {
        public string IntToRoman(int num)
        {
            int[] number = new int[]
            {
                1,4,5,9,10,40,50,90,100,400,500,900,1000
            };
            string[] Roman = new string[]
            {
                "I","IV","V","IX","X","XL","L","XC","C","CD","D","CM","M"
            };
            StringBuilder code = new StringBuilder();
            while (num > 0)
            {
                for (int i = 0; i < number.Length; i++)
                {
                    if ((i + 1) <= number.Length - 1 && num >= number[i + 1])
                    {
                        continue;
                    }
                    else
                    {
                        num -= number[i];
                        code.Append(Roman[i]);
                        break;
                    }
                }
            }

            return code.ToString();
        }
    }
}