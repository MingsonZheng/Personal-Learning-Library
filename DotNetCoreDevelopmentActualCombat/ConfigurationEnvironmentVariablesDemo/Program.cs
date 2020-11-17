using System;
using Microsoft.Extensions.Configuration;

namespace ConfigurationEnvironmentVariablesDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddEnvironmentVariables();

            var configurationRoot = builder.Build();
            Console.WriteLine($"key1:{configurationRoot["key1"]}");

            // "SECTION1__KEY3": "value3"
            // 我们定义了一个分层键 SECTION1，用双下划线隔开，这个 section 下面有一个 KEY3 的 Key
            var section = configurationRoot.GetSection("SECTION1");
            Console.WriteLine($"KEY3:{section["KEY3"]}");

            // 多级分层键
            // "SECTION1__SECTION2__KEY4": "value4"
            var section2 = configurationRoot.GetSection("SECTION1:SECTION2");
            Console.WriteLine($"KEY4:{section2["KEY4"]}");

            // 前缀过滤：是指在注入环境变量的时候，指定一个前缀，意味着只注入指定前缀的环境变量，而不是把整个操作系统的所有环境变量注入进去
            // "XIAO_KEY1": "xiao key1"
            // build 之后把读取到的环境变量的前缀去掉
            builder.AddEnvironmentVariables("XIAO_");
            configurationRoot = builder.Build();
            Console.WriteLine($"KEY1:{configurationRoot["KEY1"]}");
            // "KEY2": "value2"
            // 在注入的时候，凡是没有 XIAO_ 开头的 Key 都没有注入进来，仅注册进来需要的一个环境变量值
            // 适合当需要加载特定的值，去掉系统其他值的干扰项的场景使用
            Console.WriteLine($"KEY2:{configurationRoot["KEY2"]}");
        }
    }
}