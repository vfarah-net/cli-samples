using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HelloMvcApi.DataAccess.DataContext;
using Microsoft.AspNetCore.Mvc;

namespace HelloMvcApi
{
    [Route("/api/products")]
    public class ProductsController
    {
        //private static List<Product> _products = new List<Product>(new[] {
        //    new Product() { Id = 1, Name = "Computer" },
        //    new Product() { Id = 2, Name = "Radio" },
        //    new Product() { Id = 3, Name = "Apple" },
        //});

        private readonly IHelloDbContext helloDbContext;

        public ProductsController(IHelloDbContext helloDbContext)
        {
            this.helloDbContext = helloDbContext;
        }


        public IEnumerable<Product> Get()
        {
            //return _products;
            return helloDbContext.Products.ToList();
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            //var product = _products.FirstOrDefault(p => p.Id == id);
            var product = helloDbContext.Products.SingleOrDefault(p => p.Id == id);

            if (product == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(product);
        }

        [HttpPost]
        public void Post([FromBody]Product product)
        {
            //_products.Add(product);
            helloDbContext.Products.Add(product);
            helloDbContext.SaveChangesAsync(new CancellationToken());
        }                
    }
}