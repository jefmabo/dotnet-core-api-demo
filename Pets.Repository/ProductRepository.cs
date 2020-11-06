using System;
using System.Collections.Generic;
using System.Linq;
using Pets.Model;
using Pets.Repository.Interface;

namespace Pets.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly Context context;

        public ProductRepository(Context context) => this.context = context;

        public IList<Product> GetAll() => context.Products.ToList();
    }
}