﻿using AutoParts_ShopAndForum.Infrastructure.Data.Core;
using AutoParts_ShopAndForum.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoParts_ShopAndForum.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
        {
        }
        
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dockerConnection2 =
                    "Server=localhost;Database=AutoParts_ShopAndForum;User Id=SA;Password=SQL_SERVER_12;TrustServerCertificate=True;";
                var dockerConnection = "Server=localhost;Database=AutoParts_ShopAndForum;User Id=sa;Password=DB_pass123456;TrustServerCertificate=True;";
                var defaultConnection = "Server=DESKTOP-P07L97L\\SQLEXPRESS;Database=AutoParts_ShopAndForum;Trusted_Connection=True;TrustServerCertificate=True;";

                optionsBuilder.UseSqlServer(dockerConnection2);
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
                .ConfigureOrders()
                .ConfigurePostsCategories()
                .ConfigurePosts()
                .ConfigureComments()
                .ConfigureCourierStations();

            base.OnModelCreating(builder);
        }

        public DbSet<Town> Towns { get; set; }
        public DbSet<ProductSubcategory> ProductsSubcategories { get; set; }
        public DbSet<ProductCategory> ProductsCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderProduct> OrdersProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PostCategory> PostsCategories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CourierStation> CourierStations { get; set; }
    }
}
