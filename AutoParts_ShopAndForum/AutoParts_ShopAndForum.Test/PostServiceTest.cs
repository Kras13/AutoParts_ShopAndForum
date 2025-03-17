using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Services;
using AutoParts_ShopAndForum.Infrastructure.Data;
using AutoParts_ShopAndForum.Infrastructure.Data.Models;
using Microsoft.Extensions.DependencyInjection;

namespace AutoParts_ShopAndForum_System.Test
{
    public class PostServiceTest : IDisposable
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
                .AddSingleton<IPostService, PostService>()
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
            context.SaveChanges();
        }

        private void SeedTown()
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();

            Town town = new Town()
            {
                Name = "Stz"
            };

            context.Towns.Add(town);
            context.SaveChanges();
        }

        [Test]
        public void ArgumentExceptionWhenUsingInexistantCategory()
        {
            var postCategory = new PostCategory()
            {
                Name = "test",
                Description = "test",
                ImageUrl = "test"
            };

            var context = _serviceProvider.GetService<ApplicationDbContext>();

            context.PostsCategories.Add(postCategory);

            context.SaveChanges();

            var post = new Post()
            {
                Title = "Test",
                Content = "Test",
                CreatorId = "fake",
                CreatedOn = DateTime.Now,
                Category = postCategory,
            };

            var service = _serviceProvider.GetService<IPostService>();

            Assert.Catch<ArgumentException>(() => service.Add(post.Title, post.Content, post.PostCategoryId, post.CreatorId));
        }

        public void Dispose()
        {
            _serviceProvider?.Dispose();
        }
    }
}
