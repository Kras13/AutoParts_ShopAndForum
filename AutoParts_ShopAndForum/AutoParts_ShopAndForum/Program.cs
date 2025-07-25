using AutoParts_ShopAndForum.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using AutoParts_ShopAndForum.Areas.Forecasting;
using AutoParts_ShopAndForum.Hub;

namespace AutoParts_ShopAndForum
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var cultureInfo = new CultureInfo("en-US");
            cultureInfo.NumberFormat.NumberGroupSeparator = ".";
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            
            var stripeKey = builder.Configuration["Stripe:SecretKey"];
            Stripe.StripeConfiguration.ApiKey = stripeKey;

            builder.Services
                .AddApplicationDbContext(builder.Configuration)
                .ConfigureContextIdentity()
                .ConfigureBusinessServices();

            builder.Services.AddSignalR();

            builder.Services.AddHttpClient<ISalesForecastClient, SalesForecastClient>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ForecastApiUrl"]);
            });

            builder.Services.AddControllersWithViews()
                .AddMvcOptions(options =>
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                });

            builder.Services.AddSession();

            var app = builder.Build();

            app.SeedDatabase();

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "Areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();
            
            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}
