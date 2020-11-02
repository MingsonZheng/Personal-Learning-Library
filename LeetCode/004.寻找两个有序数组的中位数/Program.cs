using System;
using System.Threading;

namespace _004.寻找两个有序数组的中位数
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] num1 = new[] { 1, 3 };
            int[] num2 = new[] { 2 };
            var solution = new Solution();
            Console.WriteLine(solution.FindMedianSortedArrays(num1, num2));
            num1 = new[] { 1, 2 };
            num2 = new[] { 3, 4 };
            Console.WriteLine(solution.FindMedianSortedArrays(num1, num2));
        }
    }

    public class Solution
    {
        public double FindMedianSortedArrays(int[] nums1, int[] nums2)
        {
            int m = nums1.Length;
            int n = nums2.Length;
            if (m > n)
            {
                // to ensure m <= n
                int[] temp = nums1;
                nums1 = nums2;
                nums2 = temp;
                var tmp = m;
                m = n;
                n = tmp;
            }

            int iMin = 0, iMax = m, halfLen = (m + n + 1) / 2;
            while (iMin <= iMax)
            {
                int i = (iMin + iMax) / 2;
                int j = halfLen - i;
                if (i < iMax && nums2[j - 1] > nums1[i])
                {
                    iMin = i + 1;// i is too small
                }
                else if (i > iMin && nums1[i - 1] > nums2[j])
                {
                    iMax = i - 1;// i is too big
                }
                else
                {
                    // i is perfect
                    int maxLeft;
                    if (i == 0)
                    {
                        maxLeft = nums2[j - 1];
                    }
                    else if (j == 0)
                    {
                        maxLeft = nums1[i - 1];
                    }
                    else
                    {
                        maxLeft = Math.Max(nums1[i - 1], nums2[j - 1]);
                    }

                    if ((m + n) % 2 == 1)
                    {
                        return maxLeft;
                    }

                    int minRight;
                    if (i == m)
                    {
                        minRight = nums2[j];
                    }
                    else if (j == n)
                    {
                        minRight = nums1[i];
                    }
                    else
                    {
                        minRight = Math.Min(nums2[j], nums1[i]);
                    }

                    return (maxLeft + minRight) / 2.0;
                }
            }

            return 0.0;
        }
    }
}
