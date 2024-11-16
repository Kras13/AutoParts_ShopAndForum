using System.ComponentModel.DataAnnotations;

namespace AutoParts_ShopAndForum.Areas.Forum.Models
{
    public class PostInputModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public int SelectedCategoryId { get; set; }

        public PostCategoryViewModel[] Categories { get; set; }
    }
}
