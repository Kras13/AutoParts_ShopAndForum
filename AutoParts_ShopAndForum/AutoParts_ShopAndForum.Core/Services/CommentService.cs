using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Infrastructure.Data;
using AutoParts_ShopAndForum.Infrastructure.Data.Models;

namespace AutoParts_ShopAndForum.Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(string userId, int postId, string content, int? parentId = null)
        {
            var comment = new Comment()
            {
                PostId = postId,
                ParentId = parentId,
                Content = content,
                CreatedOn = DateTime.UtcNow,
                UserId = userId
            };

            _context.Add(comment);
            _context.SaveChanges();
        }
    }
}
