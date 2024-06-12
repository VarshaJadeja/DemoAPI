using Demo.Entities;
using MongoDB.Driver;

namespace Demo.Entities
{

    public class CustomsDeclarationsContext
    {
        private readonly IMongoDatabase database;

        public CustomsDeclarationsContext(IMongoDatabase database)
        {
            this.database = database;
        }

        public IMongoCollection<TEntity> GetCollection<TEntity>(string collectionName) 
            => this.database.GetCollection<TEntity>(collectionName);
    }
}
