using System.ComponentModel.DataAnnotations;

namespace AutoParts_ShopAndForum.Areas.Forum.Models
{
    public class PostInputModel
    {
        [Required]
        [Display(Name = "Заглавие")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Съдържание")]
        public string Content { get; set; }

        [Required]
        [Display(Name = "Категории")]
        public int SelectedCategoryId { get; set; }

        public PostCategoryViewModel[] Categories { get; set; }
    }
}
