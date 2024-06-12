using Demo.Entities;

using Demo;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Entities.Entities;
using System.Reflection;
using static MongoDB.Driver.WriteConcern;
using SharpCompress.Common;
using MongoDB.Bson;
using System.Xml.Linq;
using System.Linq.Expressions;

namespace Demo.Repositories.Repositories
{
    public class Repository : IRepository
    {
        private readonly IMongoCollection<ProductDetails> collection;
        private readonly IMongoCollection<ProductCategories> productCategories;
        private readonly IMongoCollection<Categories> categories;


        public Repository(CustomsDeclarationsContext context, string collectionName = null, string collectionName2 = null, string collectionName3 = null)
        {
            if (string.IsNullOrEmpty(collectionName))
            {
                collectionName = typeof(ProductDetails).Name;
            }
            if (string.IsNullOrEmpty(collectionName2))
            {
                collectionName2 = typeof(ProductCategories).Name;
            }
            if(string.IsNullOrEmpty(collectionName3))
            {
                collectionName3 = typeof(Categories).Name;
            }
            collection = context.GetCollection<ProductDetails>(collectionName);
            productCategories = context.GetCollection<ProductCategories>(collectionName2);
            categories = context.GetCollection<Categories>(collectionName3);
        }

        public async Task<List<ProductDetails>> GetAllAsync()
        {
             return await collection.Find(_ => true).ToListAsync();

        }
      
        public async Task<List<ProductDetails>> GetSearchAsync(string name)
        {
            var filter = Builders<ProductDetails>.Filter.Regex(x => x.ProductName, new BsonRegularExpression($".*{name}.*", "i"));

            return await collection.Find(filter).ToListAsync();
        }
        public async Task<ProductDetails> GetByIdAsync(string productId)
        {
            var filter = Builders<ProductDetails>.Filter.Eq("Id", productId);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task InsertAsync(ProductDetails model)
        {
            await collection.InsertOneAsync(model);
        }

        public async Task InsertManyAsync(List<ProductCategories> models)
        {
            await productCategories.InsertManyAsync(models);
        }

        public async Task UpdateAsync(string productId, ProductDetails model)
        {
            var filter = Builders<ProductDetails>.Filter.Eq("Id", productId);
            await collection.ReplaceOneAsync(filter, model);
        }
     
        public async Task DeleteOneAsync(string productId)
        {
            var filter = Builders<ProductDetails>.Filter.Eq("Id", productId);
            await collection.DeleteOneAsync(filter);
        }

        public async Task<List<Categories>> GetCategory()
        {
            return await categories.Find(_ => true).ToListAsync();

        }

    }
}
