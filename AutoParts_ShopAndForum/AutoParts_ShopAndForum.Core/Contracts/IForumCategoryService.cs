using AutoParts_ShopAndForum.Core.Models.Forum;

namespace AutoParts_ShopAndForum.Core.Contracts
{
    public interface IForumCategoryService
    {
        ForumCategoryModel[] GetAll();
        ForumCategoryModel GetById(int id);
    }
}
