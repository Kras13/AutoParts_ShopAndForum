using AutoParts_ShopAndForum.Areas.Forum.Models;
using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Areas.Forum.Controllers
{
    public class CommentController : BaseForumController
    {
        private readonly ICommentService _commentService;
        private readonly IPostService _postService;

        public CommentController(ICommentService commentService, IPostService postService)
        {
            _commentService = commentService;
            _postService = postService;
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(CommentInputModel model)
        {
            if (string.IsNullOrEmpty(model.Content)) 
            {
                TempData["IsCommentInputEmpty"] = true;

                return this.RedirectToAction("ById", "Post", new { id = model.PostId });
            }

            int? parentId = model.ParentId == 0 ? null : model.ParentId;

            if (parentId != null)
            {
                if (!_postService.ContainsComment(model.PostId, parentId.Value))
                {
                    return BadRequest();
                }
            }

            _commentService.Create(this.User.GetId(), model.PostId, model.Content, parentId);

            return this.RedirectToAction("ById", "Post", new { id = model.PostId });
        }
    }
}
