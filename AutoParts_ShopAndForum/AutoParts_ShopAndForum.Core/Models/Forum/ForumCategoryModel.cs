namespace AutoParts_ShopAndForum.Core.Models.Forum
{
    public class ForumCategoryModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public int PostsCount { get; set; }
    }
}
