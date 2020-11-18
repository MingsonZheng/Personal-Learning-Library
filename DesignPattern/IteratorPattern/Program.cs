using System;

namespace IteratorPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("迭代器模式：");
            IListCollection list = new ConcreteList();
            var iterator = list.GetIterator();

            while (iterator.MoveNext())
            {
                string i = (string)iterator.GetCurrent();
                Console.WriteLine(i);
                iterator.Next();
            }

            Console.Read();
        }
    }
}