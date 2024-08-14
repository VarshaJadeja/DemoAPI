using Demo.Entities.Entities;
using Demo.Repositories;
using Demo.Entities.ViewModels;
using Boxed.Mapping;
using AutoMapper;
using FluentResults;
using Demo.Repositories.Constants;
using MimeKit;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;


namespace Demo.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<ProductDetails> Repository;
        private readonly IRepository<ProductTable> ProductRepository;
        private readonly IRepository<Categories> CategoryRepository;
        private readonly IRepository<ProductCategories> ProductCategoryRepository;

        private readonly IMapper _mapper;
        private readonly IUserRepository UserRepository;
        private readonly IMapper<ProductDetailsViewModel, ProductDetails> ProductDetailMapper;
        private readonly SendEmailModel _emailConfig;

        public ProductService(IRepository<ProductDetails> Repository, IRepository<Categories> categoryRepository, IRepository<ProductCategories> productCategoryRepository, IUserRepository userRepository, IMapper<ProductDetailsViewModel, ProductDetails> ProductDetailMapper, IMapper mapper, SendEmailModel emailConfig, IRepository<ProductTable> ProductRepository)
        {
            this.Repository = Repository;
            this.CategoryRepository = categoryRepository;
            this.ProductCategoryRepository = productCategoryRepository;
            this.ProductRepository = ProductRepository;
            this.UserRepository = userRepository;
            this.ProductDetailMapper = ProductDetailMapper;
            _mapper = mapper;
            _emailConfig = emailConfig;
        }
        public async Task<PaginatedItemsViewModel<ProductTable>> GetPaginatedProductsAsync(int pageIndex, int pageSize)
        {
            var product = await ProductRepository.GetPaginatedProductsAsync(pageIndex, pageSize);
            return product;
        }
        public async Task<List<ProductDetails>> ProductListAsync()
        {
            return await Repository.GetAllAsync();
        }
        public async Task<List<Categories>> CategoryList()
        {
            return await CategoryRepository.GetCategory();
        }
        public async Task<Result<ProductDetails>> GetProductDetailByIdAsync(string productId)
        {
            var productDetail = await Repository.GetByIdAsync(productId);
            if(productDetail == null)
            {
                return Result.Fail<ProductDetails>(ErrorMessages.ProductNotFound);
            }
            return productDetail;
        }
        public async Task AddProductAsync(ProductDetailsViewModel productDetailsViewModel)
        {
            //BoxMapping
            //ProductDetails productDetails = new ProductDetails();
            //ProductDetailMapper.Map(productDetailsViewModel, productDetails);

            //AutoMapping
            var productDetails = _mapper.Map<ProductDetails>(productDetailsViewModel);
            await Repository.InsertAsync(productDetails);
        }
     
        public async Task UpdateProductAsync(string productId, ProductDetails productDetails)
        {
            await Repository.UpdateAsync(productId, productDetails);
        }
        public async Task DeleteProductAsync(string productId)
        {
            await Repository.DeleteOneAsync(productId);
        }

        public async Task InsertProductCategoriesAsync(List<ProductCategories> list)
        {
            await ProductCategoryRepository.InsertManyAsync(list);
        }

       
    }
}
