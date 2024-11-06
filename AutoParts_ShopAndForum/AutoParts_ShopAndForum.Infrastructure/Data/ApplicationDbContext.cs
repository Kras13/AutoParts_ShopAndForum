using AutoParts_ShopAndForum.Infrastructure.Data.Core;
using AutoParts_ShopAndForum.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoParts_ShopAndForum.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dockerConnection = "Server=localhost;Database=AutoParts_ShopAndForum;User Id=sa;Password=DB_pass123456;TrustServerCertificate=True;";
                var defaultConnection = "Server=DESKTOP-P07L97L\\SQLEXPRESS;Database=AutoParts_ShopAndForum;Trusted_Connection=True;TrustServerCertificate=True;";

                optionsBuilder.UseSqlServer(dockerConnection);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .ConfigureUsers()
                .ConfigureTowns()
                .ConfigureProductsSubcategories()
                .ConfigureProductCategories()
                .ConfigureProducts()
                .ConfigureOrdersProducts()
                .ConfigureOrders();

            base.OnModelCreating(builder);
        }

        public DbSet<Town> Towns { get; set; }
        public DbSet<ProductSubcategory> ProductsSubcategories { get; set; }
        public DbSet<ProductCategory> ProductsCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderProduct> OrdersProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
