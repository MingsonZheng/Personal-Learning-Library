using System;
using System.Threading.Tasks;
using MassTransit;

namespace mt_001
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingInMemory(sbc =>
            {
                sbc.ReceiveEndpoint("test_queue", ep =>
                {
                    ep.Handler<Message>(context => Console.Out.WriteLineAsync($"Received: {context.Message.Text}"));
                });
            });

            await bus.StartAsync();// This is important !

            await bus.Publish(new Message { Text = "Hi" });

            Console.WriteLine("Please input your message with enter:");
            string message = Console.ReadLine();

            while (message != "EXIT")
            {
                await bus.Publish(new Message() {Text = message});
                message = Console.ReadLine();
            }

            await bus.StopAsync();

            Console.WriteLine("Hello World!");
        }
    }

    public class Message
    {
        public string Text { get; set; }
    }
}
