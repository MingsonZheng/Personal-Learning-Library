using System;
using Microsoft.Extensions.FileProviders;

namespace FileProviderDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // 定义一个物理文件的提供程序，把我们当前应用程序的根目录映射出来
            IFileProvider provider1 = new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory);

            //// 获取到这个目录下面的所有内容
            //var contents = provider1.GetDirectoryContents("/");

            //foreach (var item in contents)
            //{
            //    // 读取文件流
            //    var stream = item.CreateReadStream();

            //    // 打印文件名
            //    Console.WriteLine(item.Name);
            //}

            IFileProvider provider2 = new EmbeddedFileProvider(typeof(Program).Assembly);

            var html = provider2.GetFileInfo("emb.html");

            // 传入前面的两种文件提供程序到组合提供程序里面，它可以传入多个文件提供程序
            IFileProvider provider = new CompositeFileProvider(provider1, provider2);

            var contents = provider.GetDirectoryContents("/");

            foreach (var item in contents)
            {
                Console.WriteLine(item.Name);
            }
        }
    }
}
