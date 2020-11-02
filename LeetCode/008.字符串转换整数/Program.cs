using System;

namespace _008.字符串转换整数atoi
{
    class Program
    {
        static void Main(string[] args)
        {
            string str = "42";
            var solution = new Solution();
            Console.WriteLine(solution.MyAtoi(str));
            str = "  -42";
            Console.WriteLine(solution.MyAtoi(str));
            str = "4193 with words";
            Console.WriteLine(solution.MyAtoi(str));
            str = "words and 987";
            Console.WriteLine(solution.MyAtoi(str));
            str = "-91283472332";
            Console.WriteLine(solution.MyAtoi(str));
        }
    }

    public class Solution
    {
        public int MyAtoi(string str)
        {
            int i = 0, j = 0, len = str.Length;
            bool negative = false;
            for (i = 0; i < len; i++)
            {
                if ('0' <= str.ToCharArray()[i] && str.ToCharArray()[i] <= '9')
                {
                    break;
                }
                else if (str.ToCharArray()[i] == '-' || str.ToCharArray()[i] == '+')
                {
                    negative = str.ToCharArray()[i] == '-';
                    i++;
                    break;
                }
                else if (str.ToCharArray()[i] != ' ')
                {
                    return 0;
                }
            }

            for (j = i; j < len; j++)
            {
                if (str.ToCharArray()[j] < '0' || '9' < str.ToCharArray()[j])
                {
                    break;
                }
            }

            int ret = 0;
            string num = str.Substring(i, j - i);

            for (int x = 0; x < num.Length; x++)
            {
                int cur = num.ToCharArray()[x] - '0';
                if (negative)
                {
                    if (ret < Int32.MinValue / 10 || ret == Int32.MinValue / 10 && cur > 8)
                    {
                        return Int32.MinValue;
                    }

                    ret = ret * 10 - cur;
                }
                else
                {
                    if (ret > Int32.MaxValue / 10 || ret == Int32.MaxValue / 10 && cur > 7)
                    {
                        return Int32.MaxValue;
                    }

                    ret = ret * 10 + cur;
                }
            }

            return ret;
        }
    }
}
