using System.Collections.Generic;
using Pets.Model;

namespace Pets.Service.Interface
{
    public interface IProductService
    {
        IList<Product> GetAll();
    }
}