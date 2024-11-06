using AutoParts_ShopAndForum.Infrastructure;

namespace AutoParts_ShopAndForum
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddApplicationDbContext(builder.Configuration)
                .ConfigureContextIdentity()
                .ConfigureBusinessServices();

            builder.Services.AddControllersWithViews();

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
