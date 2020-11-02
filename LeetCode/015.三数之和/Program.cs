using System;
using System.Collections.Generic;

namespace _015.三数之和
{
    class Program
    {
        static void Main(string[] args)
        {
            Solution solution = new Solution();
            var result = solution.ThreeSum(new int[] { -1, 0, 1, 2, -1, -4 });
            foreach (var re in result)
            {
                foreach (var r in re)
                {
                    Console.Write(r + ",");
                }

                Console.WriteLine();
            }
        }
    }

    public class Solution
    {
        public IList<IList<int>> ThreeSum(int[] nums)
        {
            Array.Sort(nums);
            IList<IList<int>> result = new List<IList<int>>();
            for (int i = 0; i < nums.Length - 2; i++)
            {
                if (i == 0 || i > 0 && nums[i] != nums[i - 1])
                {
                    int l = i + 1;
                    int r = nums.Length - 1;
                    while (l < r)
                    {
                        int sum = nums[i] + nums[l] + nums[r];
                        if (sum == 0)
                        {
                            result.Add(new List<int>() { nums[i], nums[l], nums[r] });
                            while (l < r && nums[l] == nums[l + 1]) l++;
                            while (l < r && nums[r] == nums[r - 1]) r--;
                            l++;
                            r--;
                        }
                        else if (sum > 0)
                        {
                            while (l < r && nums[r] == nums[r - 1]) r--;
                            r--;
                        }
                        else if (sum < 0)
                        {
                            while (l < r && nums[l] == nums[l + 1]) l++;
                            l++;
                        }
                    }
                }
            }

            return result;
        }
    }
}