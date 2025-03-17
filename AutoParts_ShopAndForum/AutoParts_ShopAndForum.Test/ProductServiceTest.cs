using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Services;
using AutoParts_ShopAndForum.Infrastructure.Data;
using AutoParts_ShopAndForum.Infrastructure.Data.Models;
using Microsoft.Extensions.DependencyInjection;

namespace AutoParts_ShopAndForum_System.Test
{
    public class ProductServiceTest : IDisposable
    {
        private ServiceProvider _serviceProvider;
        private InMemoryDbContext dbContext;

        [SetUp]
        public void Setup()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();

            _serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<IProductService, ProductService>()
                .AddSingleton<IProductSubcategoryService, ProductSubcategoryService>()
                .BuildServiceProvider();

            SeedTown();
            SeedUser();
        }

        private void SeedUser()
        {
            string adminEmail = "admin@abv.bg";
            string adminPassword = "admin123";

            User user = new User()
            {
                Email = adminEmail,
                UserName = adminEmail,
                FirstName = "Admin",
                LastName = "Adminchev",
                TownId = 1,
                PasswordHash = adminPassword
            };

            var context = _serviceProvider.GetService<ApplicationDbContext>();

            context.Users.Add(user);
        }

        private void SeedTown()
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();

            Town town = new Town()
            {
                Name = "Stz"
            };

            context.Towns.Add(town);
        }


        [Test]
        public void ArgumentExceptionWhenUsingInexistantSubcategory()
        {
            Product product = new Product()
            {
                Name = "Test",
                SubcategoryId = 1,
                CreatorId = "1",
                ImageUrl = "asdasdas",
                Price = 20,
                Description = "Test"
            };

            var service = _serviceProvider.GetService<IProductService>();

            Assert.Catch<ArgumentException>(() => service.Add(
                product.Name,
                product.Price,
                product.ImageUrl,
                product.Description,
                product.SubcategoryId,
                product.CreatorId));
        }

        [Test]
        public void ArgumentExceptionWhenInexistantUser()
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();

            ProductCategory category = new ProductCategory()
            {
                Name = "Test",
                ImageUrl = "asdasdasd"
            };

            context.ProductsCategories.Add(category);
            context.SaveChanges();

            ProductSubcategory subcategory = new ProductSubcategory()
            {
                Category = category,
                Name = "Test"
            };

            context.ProductsSubcategories.Add(subcategory);
            context.SaveChanges();

            Product product = new Product()
            {
                Name = "Test",
                Subcategory = subcategory,
                CreatorId = "does not exist",
                ImageUrl = "asdasdas",
                Price = 20,
                Description = "Test"
            };

            var service = _serviceProvider.GetService<IProductService>();

            Assert.Catch<ArgumentException>(() => service.Add(
                product.Name,
                product.Price,
                product.ImageUrl,
                product.Description,
                product.SubcategoryId,
                product.CreatorId));
        }

        [Test]
        public void SuccessfulProductAdd()
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();

            ProductCategory category = new ProductCategory()
            {
                Name = "Test",
                ImageUrl = "asdasdasd"
            };

            context.ProductsCategories.Add(category);
            context.SaveChanges();

            ProductSubcategory subcategory = new ProductSubcategory()
            {
                Category = category,
                Name = "Test"
            };

            context.ProductsSubcategories.Add(subcategory);
            context.SaveChanges();

            Assert.That(category.Id > 0);
            Assert.That(subcategory.Id > 0);

            var user = context.Users.FirstOrDefault();

            Product product = new Product()
            {
                Name = "Test",
                SubcategoryId = 1,
                CreatorId = "1",
                ImageUrl = "asdasdas",
                Price = 20,
                Description = "Test",
                Subcategory = subcategory,
                Creator = user
            };

            context.Products.Add(product);
            context.SaveChanges();

            Assert.That(product.Id > 0);
        }

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }
    }
}