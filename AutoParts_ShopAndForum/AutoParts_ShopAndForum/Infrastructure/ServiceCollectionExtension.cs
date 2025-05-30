﻿using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Services;
using AutoParts_ShopAndForum.Infrastructure.Data;
using AutoParts_ShopAndForum.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AutoParts_ShopAndForum.Infrastructure
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationDbContext(
            this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString))
                .AddDatabaseDeveloperPageExceptionFilter();

            return services;
        }

        public static IServiceCollection ConfigureContextIdentity(this IServiceCollection services)
        {
            services
                .AddDefaultIdentity<User>(
                    options =>
                    {
                        options.SignIn.RequireConfirmedAccount = false;
                        options.Password.RequireDigit = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                        options.SignIn.RequireConfirmedEmail = false;
                    })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }

        public static IServiceCollection ConfigureBusinessServices(this IServiceCollection services)
        {
            services.AddTransient<IProductCategoryService, ProductCategoryService>();
            services.AddTransient<ITownService, TownService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IProductSubcategoryService, ProductSubcategoryService>();
            services.AddTransient<ICartService, CartService>();
            services.AddTransient<IForumCategoryService, ForumCategoryService>();
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<IOrderService, OrderService>();

            return services;
        }
    }
}
