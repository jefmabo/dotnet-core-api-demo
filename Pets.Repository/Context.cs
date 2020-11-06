using Microsoft.EntityFrameworkCore;
using Pets.Model;
using Pets.Repository.Configuration;

namespace Pets.Repository
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ProductConfiguration());
        }
    }
}
