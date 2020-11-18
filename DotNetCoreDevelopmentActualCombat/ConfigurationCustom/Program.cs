using System;
using Microsoft.Extensions.Configuration;

namespace ConfigurationCustom
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            //builder.Add(new MyConfigurationSource());

            // 在定义扩展的时候，都推荐这样去做，把具体实现都定义为私有的，然后通过扩展方法的方式暴露出去
            builder.AddMyConfiguration();

            var configRoot = builder.Build();
            Console.WriteLine($"lastTime:{configRoot["lastTime"]}");

            Console.ReadKey();
        }
    }
}
