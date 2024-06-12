using Demo.Entities.Entities;
using Demo.Entities.ViewModels;

namespace Demo.Repositories
{
    public interface IProductService
    {
        public Task<List<ProductDetails>> ProductListAsync();
        public Task<ProductDetails> GetProductDetailByIdAsync(string productId);
        public Task AddProductAsync(ProductDetailsViewModel productDetailsViewModel);
        public Task<List<ProductDetails>> GetSearchAsync(string name);
        public Task UpdateProductAsync(string productId, ProductDetails productDetails);
        public Task DeleteProductAsync(String productId);

        public Task<LoginReaponce> Login(LoginRequest loginRequest);
        public bool IsUniqueUser(string username);
        public Task<User> Register(RegistrationRequest user);

        public Task InsertProductCategoriesAsync(List<ProductCategories> list);

        public Task<List<Categories>> CategoryList();
    }
}