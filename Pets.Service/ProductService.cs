using System.Collections.Generic;
using Pets.Model;
using Pets.Repository.Interface;
using Pets.Service.Interface;

namespace Pets.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;

        public ProductService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public IList<Product> GetAll() => productRepository.GetAll();
        

    }
}