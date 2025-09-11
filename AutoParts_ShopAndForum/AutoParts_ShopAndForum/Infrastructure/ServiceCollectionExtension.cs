using AutoParts_ShopAndForum.Areas.Forecasting;
using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Services;
using AutoParts_ShopAndForum.Hub;
using AutoParts_ShopAndForum.Infrastructure.Data;
using AutoParts_ShopAndForum.Infrastructure.Data.Models;
using AutoParts_ShopAndForum.Infrastructure.Options;
using AutoParts_ShopAndForum.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AutoParts_ShopAndForum.Infrastructure
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationDbContext(
            this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("Docker2Connection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString))
                .AddDatabaseDeveloperPageExceptionFilter();

            return services;
        }

        public static IServiceCollection ConfigureCustomOptions(
            this IServiceCollection services, IConfiguration config)
        {
            services.Configure<StripeOptions>(config.GetSection(StripeOptions.Section));
            services.Configure<GoogleOptions>(config.GetSection(GoogleOptions.Section));
            services.Configure<SmtpOptions>(config.GetSection(SmtpOptions.Section));
            
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
            services
                .AddTransient<IProductCategoryService, ProductCategoryService>()
                .AddTransient<ITownService, TownService>()
                .AddTransient<IProductService, ProductService>()
                .AddTransient<IProductSubcategoryService, ProductSubcategoryService>()
                .AddTransient<ICartService, CartService>()
                .AddTransient<IForumCategoryService, ForumCategoryService>()
                .AddTransient<IPostService, PostService>()
                .AddTransient<ICommentService, CommentService>()
                .AddTransient<IOrderService, OrderService>()
                .AddTransient<ICourierStationService, CourierStationService>()
                .AddTransient<IOrderNotificationService, OrderNotificationService>()
                .AddTransient<IChatService, ChatService>()
                .AddTransient<RazorViewToStringRenderer>()
                .AddTransient<IEmailSender, EmailSender>()
                .AddTransient<IActionContextAccessor, ActionContextAccessor>();

            return services;
        }

        public static IServiceCollection ConfigureGoogleAuth(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = config["Google:ClientId"];
                    options.ClientSecret = config["Google:ClientSecret"];
                });
            
            return services;
        }

        public static IServiceCollection ConfigureForecastClient(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient<ISalesForecastClient, SalesForecastClient>(client =>
            {
                client.BaseAddress = new Uri(config["ForecastApiUrl"]);
            });
            
            return services;
        }
    }
}
