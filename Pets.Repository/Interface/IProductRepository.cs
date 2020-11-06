using System.Collections.Generic;
using Pets.Model;

namespace Pets.Repository.Interface
{
    public interface IProductRepository
    {
        IList<Product> GetAll();
    }
}