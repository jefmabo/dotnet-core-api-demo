using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pets.Model;

namespace Pets.Repository.Data
{

    public static class SeedData
    {
        public static void Initialize(Context context)
        {
            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product
                    {
                        Name = "Squeaky Bone",
                        Price = 20.99m,
                    },
                    new Product
                    {
                        Name = "Knotted Rope",
                        Price = 12.99m,
                    }
                );

                context.SaveChanges();
            }
        }
    }
}