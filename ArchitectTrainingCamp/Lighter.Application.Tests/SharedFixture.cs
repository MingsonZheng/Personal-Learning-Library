using Mongo2Go;
using MongoDB.Driver;
using System.Threading.Tasks;
using Xunit;

namespace Lighter.Application.Tests
{
    public class SharedFixture:IAsyncLifetime
    {
        private MongoDbRunner _runner;
        public MongoClient Client { get; private set; }
        public IMongoDatabase Database { get; private set; }

        public async Task InitializeAsync()
        {
            _runner = MongoDbRunner.Start();
            Client = new MongoClient(_runner.ConnectionString);
            Database = Client.GetDatabase("db");

            //var hostBuilder = Program.CreateWebHostBuilder(new string[0]);
            //var host = hostBuilder.Build();
            //ServiceProvider = host.Services;
        }

        public Task DisposeAsync()
        {
            _runner?.Dispose();
            _runner = null;
            return Task.CompletedTask;
        }
    }
}
