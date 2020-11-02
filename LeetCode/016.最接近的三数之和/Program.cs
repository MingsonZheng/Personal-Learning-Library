using System;

namespace _016.最接近的三数之和
{
    class Program
    {
        static void Main(string[] args)
        {
            Solution solution = new Solution();
            Console.WriteLine(solution.ThreeSumClosest(new int[] { -1, 2, 1, -4 }, 1));
        }
    }

    public class Solution
    {
        public int ThreeSumClosest(int[] nums, int target)
        {
            Array.Sort(nums);
            var ans = nums[0] + nums[1] + nums[2];
            for (int i = 0; i < nums.Length - 2; i++)
            {
                int l = i + 1;
                int r = nums.Length - 1;
                while (l < r)
                {
                    var sum = nums[i] + nums[l] + nums[r];
                    if (Math.Abs(sum - target) < Math.Abs(ans - target)) ans = sum;
                    if (sum > target) r--;
                    else if (sum < target) l++;
                    else return ans;
                }
            }

            return ans;
        }
    }
}