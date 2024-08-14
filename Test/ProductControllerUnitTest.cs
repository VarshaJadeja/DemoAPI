using Demo.Controllers;
using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using Demo.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test
{
    public class ProductControllerUnitTest
    {
        private readonly ProductsController _controller;
        private readonly Mock<IProductService> _productService;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<ILogger<ProductsController>> _logger;
        private readonly Mock<IMemoryCache> _memoryCache;

        public ProductControllerUnitTest()
        {
            _productService = new Mock<IProductService>();
            _userServiceMock = new Mock<IUserService>();
            _memoryCache = new Mock<IMemoryCache>();
            _logger = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_productService.Object, _logger.Object, _memoryCache.Object, _userServiceMock.Object);
        }
        [Fact]
        public async Task GetPaginatedProducts_ValidRequest_ReturnsOkResult()
        {
            var pageIndex = It.IsAny<int>();
            var pageSize = It.IsAny<int>();

            var expectedProducts = new List<ProductTable>();
            var product = _productService.Setup(x => x.GetPaginatedProductsAsync(pageIndex, pageSize))
                .ReturnsAsync(new PaginatedItemsViewModel<ProductTable>(
                    status: 1,
                    message: "Paginated products retrieved successfully",
                    data: expectedProducts,
                    pageIndex: pageIndex,
                    pageSize: pageSize,
                    count: expectedProducts.Count));
         
            var response = await _controller.GetPaginatedProducts(pageIndex, pageSize);

            Assert.IsType<OkObjectResult>(response);
        }
    }
}
