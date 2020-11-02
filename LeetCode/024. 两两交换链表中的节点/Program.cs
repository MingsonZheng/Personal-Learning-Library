using System;

namespace _024._两两交换链表中的节点
{
    class Program
    {
        static void Main(string[] args)
        {
            var heard = new ListNode(1);
            var pre = new ListNode(-1);
            pre.next = heard;
            heard.next = new ListNode(2);
            heard = heard.next;
            heard.next = new ListNode(3);
            heard = heard.next;
            heard.next = new ListNode(4);
            var result = new Solution().SwapPairs(pre.next);
            Console.Write(result.val);
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
        public ListNode SwapPairs(ListNode head)
        {
            var temp = new ListNode(-1);
            temp.next = head;
            var preNode = temp;

            while (head != null && head.next != null)
            {
                var firstNode = head;
                var secondNode = head.next;

                preNode.next = secondNode;
                firstNode.next = secondNode.next;
                secondNode.next = firstNode;

                preNode = firstNode;
                head = firstNode.next;
            }

            return temp.next;
        }
    }
}