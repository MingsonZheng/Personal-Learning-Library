using System;

namespace DependencyInjectionAutofacDemo.Services
{
    public interface IMyService
    {
        void ShowCode();
    }
    public class MyService : IMyService
    {
        public void ShowCode()
        {
            Console.WriteLine($"MyService.ShowCode:{GetHashCode()}");
        }
    }

    public class MyServiceV2 : IMyService
    {
        /// <summary>
        /// 用于演示属性注入的方式
        /// </summary>
        public MyNameService NameService { get; set; }

        public void ShowCode()
        {
            // 默认情况下，NameService 为空，如果注入成功，则不为空
            Console.WriteLine($"MyServiceV2.ShowCode:{GetHashCode()},NameService是否为空：{NameService == null}");
        }
    }


    public class MyNameService
    {

    }
}