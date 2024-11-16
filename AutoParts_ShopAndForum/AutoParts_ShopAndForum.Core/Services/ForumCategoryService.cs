using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.Forum;
using AutoParts_ShopAndForum.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoParts_ShopAndForum.Core.Services
{
    public class ForumCategoryService : IForumCategoryService
    {
        private readonly ApplicationDbContext _context;

        public ForumCategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ForumCategoryModel[] GetAll()
        {
            return _context.PostsCategories
                .Include(p => p.Posts)
                .Select(x => new ForumCategoryModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl,
                    PostsCount = x.Posts.Count()
                }).ToArray();
        }
    }
}
