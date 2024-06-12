using Demo.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace Demo
{
    public static class MongoDbExtensions
    {
        public static IServiceCollection AddCustomMongoDBContext(this IServiceCollection services)
        {
            services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<ProductDBSettings>>();
                var mongoClientSettings = MongoClientSettings.FromConnectionString(options.Value.ConnectionString);
                return new MongoClient(mongoClientSettings);
            });

            return services;
        }
    }
}
