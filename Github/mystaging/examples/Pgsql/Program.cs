using System;
using MyStaging.Metadata;

namespace Pgsql
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            PgsqlDbContext dbContext = new PgsqlDbContext(new StagingOptions("Pgsql", "Host=127.0.0.1;Port=5432;Username=postgres;Password=postgres;Database=mystaging;"));
            new ContextTest().Start();

            Console.ReadKey();
            Console.WriteLine("success.....");
        }
    }
}
