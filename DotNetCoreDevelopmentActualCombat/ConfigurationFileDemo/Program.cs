using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ConfigurationFileDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();

            // 文件是否可选是它的第二个参数 optional，默认情况下是 false，这意味当文件不存在的时候它会报错
            // 它的另一个参数是 reloadOnChange， 默认情况下是 true，这意味着每次文件变更，它会去读取新文件
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            builder.AddIniFile("appsettings.ini");

            var configurationRoot = builder.Build();

            Console.WriteLine($"Key1:{configurationRoot["Key1"]}");
            Console.WriteLine($"Key2:{configurationRoot["Key2"]}");
            Console.WriteLine($"Key3:{configurationRoot["Key3"]}");
            Console.ReadKey();

            // 当我们需要追踪配置发生的变化，可以在变化发生时执行一些特定的操作
            // 配置主要提供了一个 GetReloadToken 方法，这就是跟踪配置的关键方法
            IChangeToken token = configurationRoot.GetReloadToken();

            // 注册 Callback
            token.RegisterChangeCallback(state =>
            {
                Console.WriteLine($"Key1:{configurationRoot["Key1"]}");
                Console.WriteLine($"Key2:{configurationRoot["Key2"]}");
                Console.WriteLine($"Key3:{configurationRoot["Key3"]}");
            }, configurationRoot);

            // 多次修改配置文件没有效果？
            // 因为 IChangeToken 这个对象只能使用一次，也就是说捕获到变更并且执行代码之后，需要再重新获取一个新的 IChangeToken，再次注册
            token.RegisterChangeCallback(state =>
            {
                Console.WriteLine($"Key1:{configurationRoot["Key1"]}");
                Console.WriteLine($"Key2:{configurationRoot["Key2"]}");
                Console.WriteLine($"Key3:{configurationRoot["Key3"]}");

                token = configurationRoot.GetReloadToken();
                token.RegisterChangeCallback(state2 =>
                {
                    Console.WriteLine();
                }, configurationRoot);
            }, configurationRoot);

            // 这将变成一个无限循环的过程，微软实际上提供了一个比较方便使用的快捷的扩展方法，这个方法可以帮助我们轻松地处理这件事，也就意味着每次触发完成以后可以重新绑定
            // 第一个参数是获取 IChangeToken 的方法，第二个参数是处理变更的注入方法
            ChangeToken.OnChange(() => configurationRoot.GetReloadToken(), () =>
            {
                Console.WriteLine($"Key1:{configurationRoot["Key1"]}");
                Console.WriteLine($"Key2:{configurationRoot["Key2"]}");
                Console.WriteLine($"Key3:{configurationRoot["Key3"]}");
            });

            var config = new Config()
            {
                Key1 = "config key1",
                Key5 = false,
                //Key6 = 100
            };

            //configurationRoot.Bind(config);
            //configurationRoot.GetSection("OrderService").Bind(config);
            // 可以看到 Key6 的值是100，没有发生变化，而配置中的值是200，要让私有变量生效，实际上 Bind 还有另外一个参数
            configurationRoot.GetSection("OrderService").Bind(config,
                binderOptions => { binderOptions.BindNonPublicProperties = true; });

            Console.WriteLine($"Key1:{config.Key1}");
            Console.WriteLine($"Key5:{config.Key5}");
            Console.WriteLine($"Key6:{config.Key6}");
        }
    }

    class Config
    {
        public string Key1 { get; set; }
        public bool Key5 { get; set; }
        public int Key6 { get; private set; } = 100;
    }
}
