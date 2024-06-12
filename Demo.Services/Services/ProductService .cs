using Demo.Entities.Entities;
using MongoDB.Driver;
//using Microsoft.AspNetCore.Mvc;
using Demo.Configuration;
using Demo.Entities;
using Microsoft.Extensions.Options;

using System.Collections;
using Demo.Repositories.Repositories;
using MongoDB.Bson;
using Demo.Entities.ViewModels;
using Boxed.Mapping;
using AutoMapper;


namespace Demo.Repositories
{
    public class ProductService : IProductService
    {
        private readonly IRepository Repository;
        private readonly IMapper _mapper;
        private readonly IUserRepository UserRepository;
        private readonly IMapper<ProductDetailsViewModel, ProductDetails> ProductDetailMapper;

        public ProductService(IRepository Repository, IUserRepository userRepository, IMapper<ProductDetailsViewModel, ProductDetails> ProductDetailMapper, IMapper mapper)
        {
            this.Repository = Repository;
            UserRepository = userRepository;
            this.ProductDetailMapper = ProductDetailMapper;
            _mapper = mapper;
        }
        public async Task<List<ProductDetails>> ProductListAsync()
        {
            return await Repository.GetAllAsync();
        }
        public async Task<List<Categories>> CategoryList()
        {
            return await Repository.GetCategory();
        }
        public async Task<ProductDetails> GetProductDetailByIdAsync(string productId)
        {
            return await Repository.GetByIdAsync(productId);
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

        public async Task<List<ProductDetails>> GetSearchAsync(string name)
        {
            return  await Repository.GetSearchAsync(name);
          
        }

        public async Task<LoginReaponce> Login(LoginRequest loginRequest)
        {
            var response = await UserRepository.Login(loginRequest);
            return response;
        }
        public bool IsUniqueUser(string username)
        {
            bool response =  UserRepository.IsUniqueUser(username);
            return response;
        }

        public async Task<User> Register(RegistrationRequest user)
        {
            var response = await UserRepository.Register(user);
            return response;
        }

        public async Task InsertProductCategoriesAsync(List<ProductCategories> list)
        {
            await Repository.InsertManyAsync(list);
        }
    }
}
