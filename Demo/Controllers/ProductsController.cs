using Demo.Entities.Entities;
using Demo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Demo.Entities;
using Demo.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Web.Http.Filters;
using System.Globalization;
using Demo.Entities.ViewModels;
using Demo.ExtensionMethod;
using Microsoft.Extensions.Caching.Memory;
using ExcelDataReader;
using Microsoft.AspNetCore.Cors;

namespace Demo.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors("AllowAngular")]
    //[Authorize]

    //[LogActionFilter]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly ILogger<ProductsController> _logger;
        private readonly IMemoryCache _memoryCache;
        public string cacheKey = "product";
        public ProductsController(IProductService productService, ILogger<ProductsController> logger, IMemoryCache memoryCache)
        {
           this.productService = productService;
           _logger = logger;
            _memoryCache = memoryCache;
        }

        //[HttpGet]
        ////[ResponseCache(CacheProfileName = "Default30")]
        //public IActionResult Geta()
        //{
        //    //var x = 1 / Convert.ToInt32("0", CultureInfo.InvariantCulture);
        //   //Extension Method
        //    DemoMethod dm = new DemoMethod();
        //    dm.M1();
        //    dm.M2();
        //    dm.M3();
        //    dm.M4();
        //    dm.M5("Method Name : M5");
        //    return Ok();
        //}

        [HttpGet]
        //[ResponseCache(CacheProfileName = "Default30")]
        public async Task<List<ProductDetails>> Get()
        {
            _logger.LogInformation("Seri Log is Working");
            List<ProductDetails> list;

            // Cache Service 
            //if (!_memoryCache.TryGetValue(cacheKey, out list))
            //{
            //    list = await productService.ProductListAsync();

            //    _memoryCache.Set(cacheKey, list,
            //        new MemoryCacheEntryOptions()
            //        .SetAbsoluteExpiration(TimeSpan.FromSeconds(50)));
            //}
            list = await productService.ProductListAsync();
            return list;    

        }

        [HttpGet("category")]
        //[ResponseCache(CacheProfileName = "Default30")]
        public async Task<List<Categories>> GetCategory()
        {
            List<Categories> list;
            list = await productService.CategoryList();
            return list;
        }
        [HttpGet("{productName}")]
        public async Task<List<ProductDetails>> GetSearchValue([FromQuery(Name = "productName")] string name)
        {
            return await productService.GetSearchAsync(name);
        }
        [HttpGet("{productId:length(24)}")]
    
        public async Task<ActionResult<ProductDetails>> Get(string productId)
        {

            var productDetails = await productService.GetProductDetailByIdAsync(productId);
           
            if (productDetails is null)
            {
                throw new Exception("Exception occured here...");
                //return NotFound();
            }
            return productDetails;
        }
       

        [HttpPost]
        public async Task<IActionResult> Post(ProductDetailsViewModel productDetailsViewModel)
        {
            await productService.AddProductAsync(productDetailsViewModel);
            _logger.LogInformation("New data inserted");
            return CreatedAtAction(nameof(Get), new { id = productDetailsViewModel.Id }, productDetailsViewModel);
        }


        [HttpPut("{productId:length(24)}")]
        public async Task<IActionResult> Update(string productId, ProductDetails productDetails)
        {
            var productDetail = await productService.GetProductDetailByIdAsync(productId);
            if (productDetail is null)
            {
                return NotFound();
            }
            productDetails.Id = productDetail.Id;
            await productService.UpdateProductAsync(productId, productDetails);
            return Ok();
        }

        [HttpDelete("{productId:length(24)}")]
        public async Task<IActionResult> Delete(string productId)
        {
            var productDetails = await productService.GetProductDetailByIdAsync(productId);
            if (productDetails is null)
            {
                return NotFound();
            }
            _logger.LogInformation("Data Deleted");
            await productService.DeleteProductAsync(productId);
            return Ok();
        }


        //Excel File Reader
        [Route("api/ImportProductCategoryExcelAsync")]
        [HttpPost]
        public async Task<ActionResult> ImportProductCategoryExcelAsync()
        {
            var documentcode = new List<ProductCategories>();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles", "ProductCategories.xlsx");
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // CreateReader for Excel file
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read()) //Each row of the file
                    {
                        var strId = (reader.GetValue(0) ?? "").ToString();
                        var strName = (reader.GetValue(1) ?? "").ToString();
                        var strType = (reader.GetValue(2) ?? "").ToString();
                        var strItem = (reader.GetValue(3) ?? "").ToString();
                       

                        var strAdditionalTypes = new List<string>();
                        if (strType != null)
                        {
                            strType = strType.Replace(" ", "");
                            strAdditionalTypes = strType.Split(",").ToList();
                        }

                        var declarationCategoryItem = new ProductCategories()
                        {
                            ProductId = strId,
                            ProductName = strName,
                            Type = strAdditionalTypes,
                            TotalItems = strItem,
                        };
                        documentcode.Add(declarationCategoryItem);
                    }

                    if (documentcode != null && documentcode.Count > 1)
                    {
                        documentcode.RemoveAt(0);
                        await productService.InsertProductCategoriesAsync(documentcode);
                    }
                }
            }
            return Ok();
        }
        [Route("api/ImportProductCategory1CsvAsync")]
        [HttpPost]
        public async Task<ActionResult> ImportProductCategory1CsvAsync()
        {
            var documentcode = new List<ProductCategories>();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles", "ProductCategories1.csv");
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // CreateCsvReader for Csv file
                using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
                {
                    while (reader.Read()) //Each row of the file
                    {
                        var strId = (reader.GetValue(0) ?? "").ToString();
                        var strName = (reader.GetValue(1) ?? "").ToString();
                        var strType = (reader.GetValue(2) ?? "").ToString();
                        var strItem = (reader.GetValue(3) ?? "").ToString();


                        var strAdditionalTypes = new List<string>();
                        if (strType != null)
                        {
                            strType = strType.Replace(" ", "");
                            strAdditionalTypes = strType.Split(",").ToList();
                        }

                        var declarationCategoryItem = new ProductCategories()
                        {
                            ProductId = strId,
                            ProductName = strName,
                            Type = strAdditionalTypes,
                            TotalItems = strItem,
                        };
                        documentcode.Add(declarationCategoryItem);
                    }

                    if (documentcode != null && documentcode.Count > 1)
                    {
                        documentcode.RemoveAt(0);
                        await productService.InsertProductCategoriesAsync(documentcode);
                    }
                }
            }
            return Ok();
        }
    }
}
