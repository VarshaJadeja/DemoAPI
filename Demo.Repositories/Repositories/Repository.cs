using Demo.Entities;
using MongoDB.Driver;
using FluentResults;
using Demo.Entities.ViewModels;

namespace Demo.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly IMongoCollection<T> collection;


    public Repository(CustomsDeclarationsContext context, string collectionName = null)
    {
        if (string.IsNullOrEmpty(collectionName))
        {
            collectionName = typeof(T).Name;
        }
        collection = context.GetCollection<T>(collectionName);
    }

    public async Task<List<T>> GetAllAsync()
    {
         return await collection.Find(_ => true).ToListAsync();

    }
    public async Task<PaginatedItemsViewModel<T>> GetPaginatedProductsAsync(int pageIndex, int pageSize)
    {
        var totalItems = await collection.CountDocumentsAsync( _=> true);

        var products = await collection
            .Find(_ => true)
            .Skip(pageIndex * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return new PaginatedItemsViewModel<T>(
            status: 200,
            message: "Success",
            data: products,
            pageIndex: pageIndex,
            pageSize: pageSize,
            count: totalItems
        );
    }
 
    public async Task<Result<T>> GetByIdAsync(string productId)
    {
        var filter = Builders<T>.Filter.Eq("Id", productId);
        return await collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task InsertAsync(T model)
    {
        await collection.InsertOneAsync(model);
    }

    public async Task InsertManyAsync(List<T> models)
    {
        await collection.InsertManyAsync(models);
    }

    public async Task UpdateAsync(string productId, T model)
    {
        var filter = Builders<T>.Filter.Eq("Id", productId);
        await collection.ReplaceOneAsync(filter, model);
    }
 
    public async Task DeleteOneAsync(string productId)
    {
        var filter = Builders<T>.Filter.Eq("Id", productId);
        await collection.DeleteOneAsync(filter);
    }

    public async Task<List<T>> GetCategory()
    {
        return await collection.Find(_ => true).ToListAsync();

    }

}
