using Ganss.Xss;

namespace AutoParts_ShopAndForum.Core.Models.Forum
{
    public class CommentModel
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public string SanitaziedContent
        {
            get
            {
                return new HtmlSanitizer().Sanitize(this.Content);
            }
        }

        public int? ParentId { get; set; }

        public CommentModel Parent { get; set; }

        public string CreatorUsername { get; set; }

        public string CreatedOn { get; set; }
    }
}
