using System;
using System.Text;
using RabbitMQ.Client;

namespace Sender
{
    class Sender
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello",
                        durable: false, // 持久化
                        exclusive: false, // 排它
                        autoDelete: false, // 自动删除
                        arguments: null);

                    Console.WriteLine("Please input your message with enter:");
                    string message = Console.ReadLine();
                    while (message != "EXIT")
                    {
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                            routingKey: "hello",
                            basicProperties: null,
                            body: body);
                        Console.WriteLine(" [x] Sent {0}", message);

                        Console.WriteLine("Please input your message with enter:");
                        message = Console.ReadLine();
                    }
                }

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
