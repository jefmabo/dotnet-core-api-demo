using Microsoft.Extensions.DependencyInjection;
using Pets.Repository;
using Pets.Repository.Interface;
using Pets.Service;
using Pets.Service.Interface;

namespace Pets.Api.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddLocalServices(this IServiceCollection services) 
        {

            services.AddTransient<IProductService, ProductService>();

            services.AddTransient<IProductRepository, ProductRepository>();

            return services;
        }
    }
}