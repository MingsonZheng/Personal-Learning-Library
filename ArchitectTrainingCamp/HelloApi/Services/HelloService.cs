using System;

namespace HelloApi.Services
{
    public class HelloService : IHelloService
    {
        private string _id;

        public HelloService()
        {
            _id = Guid.NewGuid().ToString();
        }

        public void Hello()
        {
            Console.WriteLine("hello dotnet core: {0}", _id);
        }
    }
}