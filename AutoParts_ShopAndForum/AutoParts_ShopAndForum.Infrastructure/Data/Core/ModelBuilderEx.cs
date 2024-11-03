using AutoParts_ShopAndForum.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoParts_ShopAndForum.Infrastructure.Data.Core
{
    internal static class ModelBuilderEx
    {
        internal static ModelBuilder ConfigureUsers(this ModelBuilder builder)
        {
            builder
                .Entity<User>()
                .HasOne(t => t.Town)
                .WithMany(t => t.Users)
                .OnDelete(DeleteBehavior.Restrict);

            //builder
            //    .Entity<User>()
            //    .HasMany(o => o.Orders)
            //    .WithOne(u => u.User)
            //    .OnDelete(DeleteBehavior.Restrict);

            //builder
            //    .Entity<User>()
            //    .HasMany(p => p.CreatedProducts)
            //    .WithOne(p => p.Creator)
            //    .OnDelete(DeleteBehavior.Restrict);

            return builder;
        }

        internal static ModelBuilder ConfigureTowns(this ModelBuilder builder)
        {
            //builder
            //    .Entity<Town>()
            //    .HasMany(u => u.Users)
            //    .WithOne(u => u.Town)
            //    .OnDelete(DeleteBehavior.Restrict);

            //builder
            //    .Entity<Town>()
            //    .HasMany(t => t.Orders)
            //    .WithOne(t => t.Town)
            //    .OnDelete(DeleteBehavior.Restrict);

            return builder;
        }

        internal static ModelBuilder ConfigureProductsSubcategories(this ModelBuilder builder)
        {
            builder
                .Entity<ProductSubcategory>()
                .HasOne(s => s.Category)
                .WithMany(s => s.Subcategories)
                .OnDelete(DeleteBehavior.Restrict);

            //builder
            //    .Entity<ProductSubcategory>()
            //    .HasMany(s => s.Products)
            //    .WithOne(s => s.Subcategory)
            //    .OnDelete(DeleteBehavior.Restrict);

            return builder;
        }

        internal static ModelBuilder ConfigureProductCategories(this ModelBuilder builder)
        {
            //builder
            //    .Entity<ProductCategory>()
            //    .HasMany(s => s.Subcategories)
            //    .WithOne(s => s.Category)
            //    .OnDelete(DeleteBehavior.Restrict);

            return builder;
        }

        internal static ModelBuilder ConfigureProducts(this ModelBuilder builder)
        {
            builder
                .Entity<Product>()
                .HasOne(u => u.Creator)
                .WithMany(u => u.CreatedProducts)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Product>()
                .HasOne(s => s.Subcategory)
                .WithMany(s => s.Products)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(14, 2);

            //builder
            //    .Entity<Product>()
            //    .HasMany(p => p.ProductOrders)
            //    .WithOne(po => po.Product)
            //    .OnDelete(DeleteBehavior.Restrict);

            return builder;
        }

        internal static ModelBuilder ConfigureOrdersProducts(this ModelBuilder builder)
        {
            builder
                .Entity<OrderProduct>()
                .HasIndex(i => new { i.ProductId, i.OrderId })
                .IsUnique();

            builder
                .Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(p => p.ProductOrders)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<OrderProduct>()
                .Property(op => op.SinglePrice)
                .HasPrecision(14, 2);

            return builder;
        }

        internal static ModelBuilder ConfigureOrders(this ModelBuilder builder)
        {
            builder
                .Entity<Order>()
                .HasOne(c => c.User)
                .WithMany(u => u.Orders)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Order>()
                .HasOne(o => o.Town)
                .WithMany(t => t.Orders)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Order>()
                .Property(o => o.OverallSum)
                .HasPrecision(14, 2);

            //builder
            //    .Entity<Order>()
            //    .HasMany(o => o.OrderProducts)
            //    .WithOne(op => op.Order)
            //    .OnDelete(DeleteBehavior.Restrict);

            return builder;
        }
    }
}
