using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace ConfigurationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // ConfigurationBuilder 是用来构建配置的核心，所有设置都在 builder 中完成
            IConfigurationBuilder builder = new ConfigurationBuilder();
            // 注入一个内存的配置数据源（注入一个字典集合作为配置数据源）
            builder.AddInMemoryCollection(new Dictionary<string, string>()
            {
                { "key1","value1" },
                { "key2","value2" },
                { "section1:key4","value4" },
                { "section2:key5","value5" },
                { "section2:key6","value6" },
                { "section2:section3:key7","value7" }
            });
            // Build 方法用来把所有的配置构建出来，并且获得一个 configurationRoot，表示配置的根
            // 也就是说读取配置的动作都需要从 IConfigurationRoot 这个对象读取的
            IConfigurationRoot configurationRoot = builder.Build();
            //IConfiguration config = configurationRoot;
            Console.WriteLine(configurationRoot["key1"]);
            Console.WriteLine(configurationRoot["key2"]);

            // section 的作用是指当配置不仅仅是简单的 Key value 的时候，比如说需要给配置分组，就可以使用 section 来定义
            // section 每一节是用冒号来作为节的分隔符的
            IConfigurationSection section = configurationRoot.GetSection("section1");
            Console.WriteLine($"key4:{section["key4"]}");
            Console.WriteLine($"key5:{section["key5"]}");

            // section1 的 key5 没有值，打印一下 section2 的 key5
            IConfigurationSection section2 = configurationRoot.GetSection("section2");
            Console.WriteLine($"key5_v2:{section2["key5"]}");

            // 多级嵌套
            var section3 = section2.GetSection("section3");
            Console.WriteLine($"key7:{section3["key7"]}");
        }
    }
}
