using System;
using System.Collections;
using System.Collections.Generic;

namespace _001.两数之和
{
    class Program
    {
        static void Main(string[] args)
        {
            var nums = new int[] { 1, 2, 3, 4, 5 };
            var solution = new Solution();

            var result = solution.TwoSum(nums, 6);
            Console.WriteLine("方法一：暴力法");
            Console.WriteLine("[" + string.Join(",", result) + "]");

            var result2 = solution.TwoSum2(nums, 6);
            Console.WriteLine("方法二：两遍字典表");
            Console.WriteLine("[" + string.Join(",", result2) + "]");

            var result3 = solution.TwoSum3(nums, 6);
            Console.WriteLine("方法三：一遍字典表");
            Console.WriteLine("[" + string.Join(",", result3) + "]");
        }
    }

    public class Solution
    {
        /// <summary>
        /// 方法一：暴力法
        /// </summary>
        /// <param name="nums"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public int[] TwoSum(int[] nums, int target)
        {
            for (int i = 0; i < nums.Length; i++)
            {
                for (int j = i + 1; j < nums.Length; j++)
                {
                    if (nums[j] == target - nums[i])
                    {
                        return new int[] { i, j };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 方法二：两遍字典表
        /// </summary>
        /// <param name="nums"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public int[] TwoSum2(int[] nums, int target)
        {
            var dic = new Dictionary<int, int>();
            for (int i = 0; i < nums.Length; i++)
            {
                if (!dic.ContainsKey(nums[i]))
                {
                    dic.Add(nums[i], i);
                }
            }

            for (int i = 0; i < nums.Length; i++)
            {
                var complement = target - nums[i];
                if (dic.ContainsKey(complement) && dic[complement] != i)
                {
                    return new int[] { i, dic[complement] };
                }
            }

            return null;
        }

        /// <summary>
        /// 方法三：一遍字典表
        /// </summary>
        /// <param name="nums"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public int[] TwoSum3(int[] nums, int target)
        {
            var dic = new Dictionary<int, int>();
            for (int i = 0; i < nums.Length; i++)
            {
                int complemnt = target - nums[i];
                if (dic.ContainsKey(complemnt) && dic[complemnt] != i)
                {
                    return new int[] { dic[complemnt], i };
                }
                if (!dic.ContainsKey(nums[i]))
                {
                    dic.Add(nums[i], i);
                }
            }

            return null;
        }
    }
}
