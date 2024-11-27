namespace AutoParts_ShopAndForum.Core.Contracts
{
    public interface ICommentService
    {
        void Create(string userId, int postId, string content, int? parentId = null);
    }
}
