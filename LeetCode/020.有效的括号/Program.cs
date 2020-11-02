using System;
using System.Collections.Generic;

namespace _020.有效的括号
{
    class Program
    {
        static void Main(string[] args)
        {
            Solution solution = new Solution();
            Console.WriteLine(solution.IsValid("()"));
            Console.WriteLine(solution.IsValid("()[]{}"));
            Console.WriteLine(solution.IsValid("(]"));
            Console.WriteLine(solution.IsValid("([)]"));
            Console.WriteLine(solution.IsValid("{[]}"));
        }
    }

    public class Solution
    {
        public bool IsValid(string s)
        {
            Dictionary<char, char> dic = new Dictionary<char, char>
            {
                {'}', '{'},
                {']', '['},
                {')', '('},
            };

            Stack<char> stack = new Stack<char>();

            foreach (var c in s)
            {
                if (!dic.ContainsKey(c))
                {
                    stack.Push(c);
                }
                else
                {
                    if (stack.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        var a = stack.Pop();
                        if (dic[c] != a)
                        {
                            return false;
                        }
                    }
                }
            }

            if (stack.Count != 0)
            {
                return false;
            }

            return true;
        }
    }
}