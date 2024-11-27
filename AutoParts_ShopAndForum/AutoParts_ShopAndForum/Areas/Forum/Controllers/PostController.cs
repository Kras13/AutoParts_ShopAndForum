using AutoParts_ShopAndForum.Areas.Forum.Models;
using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Areas.Forum.Controllers
{
    public class PostController : BaseForumController
    {
        private readonly IPostService _postService;
        private readonly IForumCategoryService _categoryService;

        public PostController(IPostService postService, IForumCategoryService categoryService)
        {
            this._postService = postService;
            _categoryService = categoryService;
        }

        public IActionResult List(int categoryId)
        {
            var model = _postService
                .GetByCategoryId(categoryId)
                .Select(m => new PostListViewModel()
                {
                    Id = m.Id,
                    Author = m.CreatorUsername,
                    DateCreate = m.CreatedOn,
                    Title = m.Title,
                    CommentsCount = m.Comments.Length
                }).ToArray();

            return View(model);
        }

        public IActionResult ById(int id)
        {
            var post = _postService.GetById(id);

            ViewBag.IsCommentInputEmpty = TempData["IsCommentInputEmpty"];

            return View(post);
        }

        [Authorize]
        public IActionResult Create()
        {
            var model = new PostInputModel()
            {
                Categories = _categoryService
                .GetAll()
                .Select(c => new PostCategoryViewModel()
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToArray()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(PostInputModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = _categoryService
                    .GetAll().
                    Select(c => new PostCategoryViewModel()
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToArray();

                return View(model);
            }

            _postService.Add(
                model.Title, model.Content, model.SelectedCategoryId, this.User.GetId());

            return RedirectToAction("All", "Category", new { area = "Forum" });
        }
    }
}
