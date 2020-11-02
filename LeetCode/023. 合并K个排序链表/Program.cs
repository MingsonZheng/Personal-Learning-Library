using System;

namespace _023._合并K个排序链表
{
    class Program
    {
        static void Main(string[] args)
        {
            var heard1 = new ListNode(1);
            var pre1 = heard1;
            heard1.next = new ListNode(4);
            heard1 = heard1.next;
            heard1.next = new ListNode(5);

            var heard2 = new ListNode(1);
            var pre2 = heard2;
            heard2.next = new ListNode(3);
            heard2 = heard2.next;
            heard2.next = new ListNode(4);

            var heard3 = new ListNode(2);
            var pre3 = heard3;
            heard3.next = new ListNode(6);

            var lists = new ListNode[]
            {
                pre1, pre2, pre3,
            };

            var result = new Solution().MergeKLists(lists);
            if (result != null)
            {
                Console.Write(result.val);
            }

            while (result.next != null)
            {
                Console.Write($"->{result.next.val}");
                result = result.next;
            }
        }
    }

    public class ListNode
    {
        public int val;
        public ListNode next;
        public ListNode(int x) { val = x; }
    }

    public class Solution
    {
        public ListNode MergeKLists(ListNode[] lists)
        {
            if (lists == null || lists.Length == 0)
                return null;

            return MergeList(lists, 0, lists.Length - 1);
        }

        public ListNode MergeList(ListNode[] lists, int start, int end)
        {
            if (start >= end)
            {
                return lists[start];
            }

            var mid = start + (end - start) / 2;
            var l1 = MergeList(lists, start, mid);
            var l2 = MergeList(lists, mid + 1, end);
            return MergeTwoList(l1, l2);
        }

        public ListNode MergeTwoList(ListNode l1, ListNode l2)
        {
            var heard = new ListNode(0);
            var pre = heard;

            while (l1 != null && l2 != null)
            {
                if (l1.val <= l2.val)
                {
                    pre.next = l1;
                    l1 = l1.next;
                    pre = pre.next;
                }
                else
                {
                    pre.next = l2;
                    l2 = l2.next;
                    pre = pre.next;
                }
            }

            if (l1 == null)
            {
                pre.next = l2;
            }
            else
            {
                pre.next = l1;
            }

            return heard.next;
        }
    }
}
