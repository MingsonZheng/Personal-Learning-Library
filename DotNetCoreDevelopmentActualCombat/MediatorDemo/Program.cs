using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddMediatR(typeof(Program).Assembly);

            var serviceProvider = services.BuildServiceProvider();

            var mediator = serviceProvider.GetService<IMediator>();

            //await mediator.Send(new MyCommand { CommandName = "cmd01" });
            await mediator.Publish(new MyEvent { EventName = "event01" });
        }
    }

    internal class MyCommand : IRequest<long>
    {
        public string CommandName { get; set; }
    }

    //internal class MyCommandHandlerV2 : IRequestHandler<MyCommand, long>
    //{
    //    public Task<long> Handle(MyCommand request, CancellationToken cancellationToken)
    //    {
    //        Console.WriteLine($"MyCommandHandlerV2执行命令：{request.CommandName}");
    //        return Task.FromResult(10L);
    //    }
    //}

    internal class MyCommandHandler : IRequestHandler<MyCommand, long>
    {
        public Task<long> Handle(MyCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"MyCommandHandler执行命令：{request.CommandName}");
            return Task.FromResult(10L);
        }
    }

    internal class MyCommandHandlerV2 : IRequestHandler<MyCommand, long>
    {
        public Task<long> Handle(MyCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"MyCommandHandlerV2执行命令：{request.CommandName}");
            return Task.FromResult(10L);
        }
    }

    internal class MyEvent : INotification
    {
        public string EventName { get; set; }
    }

    internal class MyEventHandler : INotificationHandler<MyEvent>
    {
        public Task Handle(MyEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"MyEventHandler执行：{notification.EventName}");
            return Task.CompletedTask;
        }
    }

    internal class MyEventHandlerV2 : INotificationHandler<MyEvent>
    {
        public Task Handle(MyEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"MyEventHandlerV2执行：{notification.EventName}");
            return Task.CompletedTask;
        }
    }
}
