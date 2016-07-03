using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HelloMvcApi.DataAccess.DataContext
{
    public class HelloDbContext : DbContext, IHelloDbContext
    {
        public HelloDbContext()
            : base()
        { }

        public HelloDbContext(DbContextOptions<HelloDbContext> options)
            :base(options)
        { }
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Startup.Configuration.GetConnectionString("DefaultConnection"));
            base.OnConfiguring(optionsBuilder);
        }
    }
}
