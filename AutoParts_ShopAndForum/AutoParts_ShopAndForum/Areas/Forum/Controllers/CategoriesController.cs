using AutoParts_ShopAndForum.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Areas.Forum.Controllers
{
    public class CategoryController : BaseForumController
    {
        private readonly IForumCategoryService _categoryService;

        public CategoryController(IForumCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IActionResult All()
        {
            var model = _categoryService.GetAll();

            return View(model);
        }
    }
}
