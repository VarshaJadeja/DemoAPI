using Demo.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Repositories.Repositories
{
    public interface IRepository
    {
        public Task<List<ProductDetails>> GetAllAsync();

        public Task<ProductDetails> GetByIdAsync(string productId);

        public Task InsertAsync(ProductDetails model);

        public Task UpdateAsync(string productId, ProductDetails model);

        public Task InsertManyAsync(List<ProductCategories> models);

        public Task DeleteOneAsync(string productId);

        public Task<List<ProductDetails>> GetSearchAsync(string name);

        public Task<List<Categories>> GetCategory();
    }
}
