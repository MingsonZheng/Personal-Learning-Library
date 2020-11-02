using System;

namespace _019.删除链表的倒数第N个节点
{
    class Program
    {
        static void Main(string[] args)
        {
            ListNode head = new ListNode(1);
            head.next = new ListNode(2);
            head.next.next = new ListNode(3);
            head.next.next.next = new ListNode(4);
            head.next.next.next.next = new ListNode(5);
            Solution solution = new Solution();
            ListNode result = solution.RemoveNthFromEnd(head, 2);
            Console.WriteLine(result.val);
            Console.WriteLine(result.next.val);
            Console.WriteLine(result.next.next.val);
            Console.WriteLine(result.next.next.next.val);
        }
    }

    // Definition for singly-linked list.
    public class ListNode
    {
        public int val;
        public ListNode next;
        public ListNode(int x) { val = x; }
    }

    public class Solution
    {
        public ListNode RemoveNthFromEnd(ListNode head, int n)
        {
            ListNode result = new ListNode(0);
            result.next = head;
            ListNode first = result;
            ListNode second = result;
            for (int i = 1; i <= n + 1; i++)
                first = first.next;
            while (first != null)
            {
                first = first.next;
                second = second.next;
            }

            second.next = second.next.next;

            return result.next;
        }
    }
}