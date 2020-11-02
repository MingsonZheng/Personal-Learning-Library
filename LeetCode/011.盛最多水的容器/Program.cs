using System;

namespace _011.盛最多水的容器
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] height = new int[]
            {
                1,8,6,2,5,4,8,3,7
            };
            var solution = new Solution();
            Console.WriteLine(solution.MaxArea(height));
        }
    }

    public class Solution
    {
        public int MaxArea(int[] height)
        {
            int maxArea = 0, l = 0, r = height.Length - 1;
            while (l < r)
            {
                maxArea = Math.Max(maxArea, Math.Min(height[l], height[r]) * (r - l));
                if (height[l] < height[r])
                {
                    l++;
                }
                else
                {
                    r--;
                }
            }

            return maxArea;
        }
    }
}