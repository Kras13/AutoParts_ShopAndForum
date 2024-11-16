using AutoParts_ShopAndForum.Core.Models.Forum;

namespace AutoParts_ShopAndForum.Core.Contracts
{
    public interface IPostService
    {
        void Add(string title, string content, int categoryId, string creatorId);

        PostModel GetById(int id);

        PostModel[] GetByCategoryId(int id);

        bool ContainsComment(int postId, int commentId);
    }
}
