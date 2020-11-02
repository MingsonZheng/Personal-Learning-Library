using System;
using System.Collections.Generic;

namespace _018.四数之和
{
    class Program
    {
        static void Main(string[] args)
        {
            var solution = new Solution();
            //var result = solution.FourSum(new int[] { 1, 0, -1, 0, -2, 2 }, 0);
            //var result = solution.FourSum(new int[] { 0, 0, 0, 0 }, 0);
            //var result = solution.FourSum(new int[] {-3, -2, -1, 0, 0, 1, 2, 3}, 0);
            var result = solution.FourSum(new int[] { -2, -1, 0, 0, 1, 2 }, 0);
            foreach (var item in result)
            {
                Console.Write(string.Join(",", item));
                Console.WriteLine();
            }
        }
    }

    public class Solution
    {
        public IList<IList<int>> FourSum(int[] nums, int target)
        {
            Array.Sort(nums);
            IList<IList<int>> result = new List<IList<int>>();
            for (int i = 0; i < nums.Length - 3; i++)
            {
                if (i == 0 || i > 0 && nums[i] != nums[i - 1])
                {
                    for (int j = i + 1; j < nums.Length - 2; j++)
                    {
                        if (j == i + 1 || j > i + 1 && nums[j] != nums[j - 1])
                        {
                            int l = j + 1;
                            int r = nums.Length - 1;
                            while (l < r)
                            {
                                int sum = nums[i] + nums[j] + nums[l] + nums[r];
                                if (sum == target)
                                {
                                    result.Add(new List<int>() { nums[i], nums[j], nums[l], nums[r] });
                                    while (l < r && nums[l] == nums[l + 1]) l++;
                                    while (l < r && nums[r] == nums[r - 1]) r--;
                                    l++;
                                    r--;
                                }
                                else if (sum > target)
                                {
                                    while (l < r && nums[r] == nums[r - 1]) r--;
                                    r--;
                                }
                                else if (sum < target)
                                {
                                    while (l < r && nums[l] == nums[l + 1]) l++;
                                    l++;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
