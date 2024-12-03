using AutoParts_ShopAndForum.Infrastructure.Data;
using AutoParts_ShopAndForum.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AutoParts_ShopAndForum.Infrastructure
{
    public static class ApplicationsBuilderExtension
    {
        public static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
        {
            Task.Run(async () =>
            {
                using (
                    IServiceScope serviceScope = app.ApplicationServices
                        .GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                {
                    var dbContext = serviceScope.ServiceProvider
                        .GetService<ApplicationDbContext>();

                    if (dbContext == null)
                        throw new Exception("ApplicationDbContext is not configured.");

                    dbContext.Database.Migrate();

                    await SeedTowns(dbContext);
                    await dbContext.SaveChangesAsync();

                    await SeedAdministrator(serviceScope.ServiceProvider, dbContext);
                    await dbContext.SaveChangesAsync();

                    await SeedSeller(serviceScope.ServiceProvider, dbContext);
                    await dbContext.SaveChangesAsync();

                    await SeedProductCategories(dbContext);
                    await dbContext.SaveChangesAsync();

                    await SeedSubcategories(serviceScope.ServiceProvider, dbContext);
                    await dbContext.SaveChangesAsync();

                    await SeedForumCategories(dbContext);
                    await dbContext.SaveChangesAsync();
                }
            });

            return app;
        }

        private static async Task SeedForumCategories(ApplicationDbContext dbContext)
        {
            var savedCategories = await dbContext.PostsCategories.ToArrayAsync();

            var newCategories = new PostCategory[]
            {
                new PostCategory(){
                    Name = "Best oils",
                    Description = "A place where the oils can be reviewed",
                    ImageUrl = "https://d2hucwwplm5rxi.cloudfront.net/wp-content/uploads/2021/08/24094058/Wrong-Engine-Oil-080420210237.jpg"},
                new PostCategory(){
                    Name = "Best filters",
                    Description = "A place where the filters can be reviewed",
                    ImageUrl = "https://s19528.pcdn.co/wp-content/uploads/2018/05/Air-and-Oil-Filters-Automotive.jpg"},
                new PostCategory(){
                    Name = "Best timing belts",
                    Description = "A place where the timing belts can be reviewed",
                    ImageUrl = "https://www.kmotorshop.com/document/shop/CT1168K1/CT1168K1A.jpg"},
            };

            foreach (var category in newCategories)
            {
                if (savedCategories.Any(c => c.Name == category.Name))
                {
                    continue;
                }

                await dbContext.PostsCategories.AddAsync(category);
            }
        }

        private static async Task SeedSubcategories(IServiceProvider serviceProvider, ApplicationDbContext dbContext)
        {
            var categories = await dbContext.ProductsCategories.ToArrayAsync();

            var subcategories = new ProductSubcategory[]
                {
                    new ProductSubcategory(){Name = "Oils", CategoryId = categories[0].Id},
                    new ProductSubcategory(){Name = "Antifreeze", CategoryId = categories[0].Id},
                    new ProductSubcategory(){Name = "Oil filters", CategoryId = categories[1].Id},
                    new ProductSubcategory(){Name = "Air filters", CategoryId = categories[1].Id},
                    new ProductSubcategory(){Name = "Front window", CategoryId = categories[2].Id},
                    new ProductSubcategory(){Name = "Back window", CategoryId = categories[2].Id},
                    new ProductSubcategory(){Name = "Oil additives", CategoryId = categories[3].Id},
                    new ProductSubcategory(){Name = "Coolant additives", CategoryId = categories[3].Id},
                };

            var savedSubcategories = await dbContext.ProductsSubcategories.ToArrayAsync();

            foreach (var subCat in subcategories)
            {
                if (savedSubcategories.Any(c => c.Name == subCat.Name))
                {
                    continue;
                }

                await dbContext.ProductsSubcategories.AddAsync(new ProductSubcategory()
                {
                    Name = subCat.Name,
                    CategoryId = subCat.CategoryId
                });
            }
        }

        private static async Task SeedProductCategories(ApplicationDbContext context)
        {
            var categories = new ProductCategory[]
                {
                    new ProductCategory() {Name = "Oils and liquids", ImageUrl = "https://www.autopower.bg/images/categories/%D0%9C%D0%B0%D1%81%D0%BB%D0%B0%20%D0%B8%20%D1%82%D0%B5%D1%87%D0%BD%D0%BE%D1%81%D1%82%D0%B8.jpg"},
                    new ProductCategory() {Name = "Filters", ImageUrl = "https://www.autopower.bg/images/categories/%D0%A4%D0%B8%D0%BB%D1%82%D1%80%D0%B8.jpg" },
                    new ProductCategory() {Name = "Windows cleaning", ImageUrl = "https://www.autopower.bg/images/categories/%D0%9F%D0%BE%D1%87%D0%B8%D1%81%D1%82%D0%B2%D0%B0%D0%BD%D0%B5%20%D0%BD%D0%B0%20%D1%81%D1%82%D1%8A%D0%BA%D0%BB%D0%B0%D1%82%D0%B0.jpg" },
                    new ProductCategory() {Name = "Additives", ImageUrl = "https://fanshop.vwclub.bg/xprod/155.jpg"}
                };

            var savedCategories = await context.ProductsCategories.ToArrayAsync();

            foreach (var category in categories)
            {
                if (savedCategories.Any(c => c.Name == category.Name))
                {
                    continue;
                }

                await context.ProductsCategories.AddAsync(new ProductCategory()
                {
                    Name = category.Name,
                    ImageUrl = category.ImageUrl
                });
            }
        }

        private static async Task SeedSeller(IServiceProvider serviceProvider, ApplicationDbContext dbContext)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            var sellerRoleExists = await roleManager
                .RoleExistsAsync(RoleType.Seller);

            if (sellerRoleExists)
            {
                return;
            }

            var result = await roleManager.CreateAsync(new IdentityRole(RoleType.Seller));

            if (result.Succeeded)
            {
                string sellerEmail = "seller@abv.bg";
                string sellerPassword = "123456";

                var user = new User()
                {
                    Email = sellerEmail,
                    UserName = sellerEmail,
                    FirstName = "Seller",
                    LastName = "Sellerov",
                    TownId = dbContext.Towns.ToArray()[1].Id,
                };

                await userManager.CreateAsync(user, sellerPassword);
                await userManager.AddToRoleAsync(user, RoleType.Seller);
            }
        }

        private static async Task SeedAdministrator(IServiceProvider serviceProvider, ApplicationDbContext dbContext)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            var adminRoleExists = await roleManager
                .RoleExistsAsync(RoleType.Administrator);

            if (adminRoleExists)
            {
                return;
            }

            var roleCreated = await roleManager.CreateAsync(new IdentityRole(RoleType.Administrator));

            if (roleCreated.Succeeded)
            {
                var adminEmail = "admin@abv.bg";
                var adminPassword = "admin123";

                var user = new User()
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    FirstName = "Admin",
                    LastName = "Adminchev",
                    TownId = dbContext.Towns.ToArray()[1].Id,
                };

                await userManager.CreateAsync(user, adminPassword);

                await userManager.AddToRoleAsync(user, RoleType.Administrator);
            }
        }

        private static async Task SeedTowns(ApplicationDbContext context)
        {
            var towns = new string[] { "Stara Zagora", "Sofia", "Varna", "Plovdiv", "Burgas", "Pleven" };
            var savedTowns = await context.Towns.ToArrayAsync();

            foreach (var town in towns)
            {
                if (savedTowns.Any(t => t.Name == town))
                {
                    continue;
                }

                await context.Towns.AddAsync(new Town() { Name = town });
            }
        }
    }
}
