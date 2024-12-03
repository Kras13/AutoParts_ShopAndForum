using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.Forum;
using AutoParts_ShopAndForum.Infrastructure.Data;
using AutoParts_ShopAndForum.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoParts_ShopAndForum.Core.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(string title, string content, int categoryId, string creatorId)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == creatorId);

            if (user == null)
                throw new InvalidOperationException("User with such id does not exist.");

            var category = _context.PostsCategories.FirstOrDefault(x => x.Id == categoryId);

            if(category == null)
                throw new InvalidOperationException("Category with such id does not exist.");

            var post = new Post()
            {
                Title = title,
                Content = content,
                CreatedOn = DateTime.UtcNow,
                PostCategoryId = categoryId,
                CreatorId = creatorId
            };

            _context.Add(post);
            _context.SaveChanges();
        }

        public PostModel GetById(int id)
        {
            var post = _context.Posts
                .Include(p => p.Comments)
                .ThenInclude(cp => cp.User)
                .Include(c => c.Creator)
                .FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                throw new ArgumentException("Post with such id was not found.");
            }

            var comments = new List<CommentModel>();

            foreach (var comment in post.Comments)
            {
                CommentModel parent = GetCurrentCommentParent(comment, comments);

                comments.Add(new CommentModel()
                {
                    Id = comment.Id,
                    ParentId = comment.ParentId,
                    Parent = parent,
                    Content = comment.Content,
                    CreatorUsername = comment.User.UserName,
                    CreatedOn = comment.CreatedOn.ToString("dd/MM/yyyy")
                });
            }

            return new PostModel()
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Comments = comments.ToArray(),
                CreatedOn = post.CreatedOn.ToString("dd/MM/yyyy"),
                CreatorUsername = post.Creator.UserName
            };
        }

        public PostModel[] GetByCategoryId(int id)
        {
            return _context.Posts
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .Where(e => e.PostCategoryId == id)
                .Select(e => new PostModel()
                {
                    Id = e.Id,
                    Title = e.Title,
                    Content = e.Content,
                    CreatedOn = e.CreatedOn.ToShortDateString(),
                    CreatorUsername = e.Creator.UserName,
                    Comments = e.Comments.Select(c =>
                        new CommentModel()
                        {
                            Id = c.Id,
                            ParentId = c.ParentId,
                            Content = c.Content,
                            CreatedOn = c.CreatedOn.ToShortDateString(),
                            CreatorUsername = c.User.UserName
                        }).ToArray()
                }).ToArray();
        }

        public bool ContainsComment(int postId, int commentId)
        {
            var post = _context.Posts
                .Include(p => p.Comments)
                .FirstOrDefault(e => e.Id == postId);

            if (post == null)
                throw new ArgumentException("Post with such id does not exist");

            return post.Comments.FirstOrDefault(e => e.Id == commentId) != null;
        }

        private CommentModel GetCurrentCommentParent(Comment comment, IList<CommentModel> comments)
        {
            if (comment.Parent == null)
            {
                return null;
            }

            var parentReference = comments.FirstOrDefault(n => n.Id == comment.Parent.Id);

            return parentReference;
        }
    }
}
