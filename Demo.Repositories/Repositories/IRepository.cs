using Demo.Entities.ViewModels;
using FluentResults;

namespace Demo.Repositories;

public interface IRepository<T> where T : class
{
    public Task<PaginatedItemsViewModel<T>> GetPaginatedProductsAsync(int pageIndex, int pageSize);

    public Task<List<T>> GetAllAsync();

    public Task<Result<T>> GetByIdAsync(string productId);

    public Task InsertAsync(T model);

    public Task UpdateAsync(string productId, T model);

    public Task InsertManyAsync(List<T> models);

    public Task DeleteOneAsync(string productId);

    //public Task<List<T>> GetSearchAsync(string name);

    public Task<List<T>> GetCategory();
}
