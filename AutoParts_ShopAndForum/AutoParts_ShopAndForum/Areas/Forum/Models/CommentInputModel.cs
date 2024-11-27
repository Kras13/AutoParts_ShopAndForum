using System.ComponentModel.DataAnnotations;

namespace AutoParts_ShopAndForum.Areas.Forum.Models
{
    public class CommentInputModel
    {
        public int PostId { get; set; }

        public int ParentId { get; set; }

        public string Content { get; set; }
    }
}
