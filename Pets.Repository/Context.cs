using Microsoft.EntityFrameworkCore;
using Pets.Model;

namespace Pets.Repository
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }        
    }
}
