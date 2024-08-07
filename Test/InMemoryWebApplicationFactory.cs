using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Mongo2Go;

namespace Test;

public class InMemoryWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private MongoDbRunner _mongoDbRunner;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing")
               .ConfigureServices(services =>
               {
                   _mongoDbRunner = MongoDbRunner.Start();

                   services.AddScoped<IMongoClient>(provider =>
                   {
                       return new MongoClient(_mongoDbRunner.ConnectionString);
                   });

                   services.AddScoped(sp =>
                   {
                       var client = sp.GetRequiredService<IMongoClient>();
                       return client.GetDatabase("TestDatabase");
                   });
               });

        AppDomain.CurrentDomain.ProcessExit += (sender, e) => _mongoDbRunner?.Dispose();
    }
}
