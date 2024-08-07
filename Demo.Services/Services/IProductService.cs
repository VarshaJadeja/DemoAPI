using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using FluentResults;

namespace Demo.Services.Services;

public interface IProductService
{
    public Task<PaginatedItemsViewModel<ProductTable>> GetPaginatedProductsAsync(int pageIndex, int pageSize);
    public Task<List<ProductDetails>> ProductListAsync();
    public Task<Result<ProductDetails>> GetProductDetailByIdAsync(string productId);
    public Task AddProductAsync(ProductDetailsViewModel productDetailsViewModel);
    //public Task<List<ProductDetails>> GetSearchAsync(string name);
    public Task UpdateProductAsync(string productId, ProductDetails productDetails);
    public Task DeleteProductAsync(String productId);
    public Task InsertProductCategoriesAsync(List<ProductCategories> list);
    public Task<List<Categories>> CategoryList();

}