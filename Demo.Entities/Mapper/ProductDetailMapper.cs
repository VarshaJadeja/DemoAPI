
using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using Boxed.Mapping;

namespace Demo.Entities.Mapper
{
    public class ProductDetailMapper : IMapper<ProductDetailsViewModel, ProductDetails>
    {
        // BoxMapping
        public void Map(ProductDetailsViewModel source, ProductDetails destination)
        {
            destination.Id = source.Id;
            destination.ProductName = source.ProductName;
            destination.ProductDescription = source.ProductDescription;
            destination.ProductPrice = source.ProductPrice;
            destination.ProductStock = source.ProductStock;
            destination.CategoryId = source.CategoryId;
            //destination.productType = source.productType;
        }
    }
}
