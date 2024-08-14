using Bogus;
using Demo.Entities.Entities;
using MongoDB.Driver;

namespace Test
{
    public class DatabaseSetup
    {
        public static void Seed(IMongoDatabase database)
        {
            // Get the collection
            var collection = database.GetCollection<ProductDetails>("ProductDetails");

            // Create a Faker for ProductDetails
            var productFaker = new Faker<ProductDetails>()
                .RuleFor(p => p.Id, f => "") // Generate a unique ID
                .RuleFor(p => p.ProductName, f => f.Commerce.ProductName())
                .RuleFor(p => p.ProductStock, f => f.Random.Int(1, 100))
                .RuleFor(p => p.ProductDescription, f => f.Commerce.ProductDescription());

            // Generate fake products
            var products = productFaker.Generate(10); 

            collection.InsertMany(products);
        }
        public static void SeedUser(IMongoDatabase database)
        {
            var collection2 = database.GetCollection<User>("User");
            var users = new List<User>
            {
                new User { Id = "", Password = "123", UserName = "123"}
            };
            collection2.InsertMany(users);
        }
    }
}
