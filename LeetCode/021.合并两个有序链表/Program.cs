using System;

namespace _021.合并两个有序链表
{
    class Program
    {
        static void Main(string[] args)
        {
            var solution = new Solution();
            var l1 = new ListNode(1);
            var l11 = l1;
            l11.next = new ListNode(2);
            l11 = l11.next;
            l11.next = new ListNode(4);
            var l2 = new ListNode(1);
            var l22 = l2;
            l22.next = new ListNode(3);
            l22 = l22.next;
            l22.next = new ListNode(4);
            var result = solution.MergeTwoLists(l1, l2);
            Console.Write(result.val);
            result = result.next;
            while (result != null)
            {
                Console.Write("->");
                Console.Write(result.val);
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
        public ListNode MergeTwoLists(ListNode l1, ListNode l2)
        {
            var head = new ListNode(0);
            var currentNode = head;

            while (l1 != null && l2 != null)
            {
                if (l1.val < l2.val)
                {
                    currentNode.next = l1;
                    l1 = l1.next;
                }
                else
                {
                    currentNode.next = l2;
                    l2 = l2.next;
                }

                currentNode = currentNode.next;
            }

            if (l1 != null)
            {
                currentNode.next = l1;
            }
            else
            {
                currentNode.next = l2;
            }

            return head.next;
        }
    }
}