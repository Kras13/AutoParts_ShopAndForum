using Ganss.Xss;

namespace AutoParts_ShopAndForum.Core.Models.Forum
{
    public class PostModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string SanitaziedContent
        {
            get
            {
                return new HtmlSanitizer().Sanitize(this.Content);
            }
        }
        public CommentModel[] Comments { get; set; }
        public string CreatorUsername { get; set; }
        public string CreatedOn { get; set; }
    }
}
