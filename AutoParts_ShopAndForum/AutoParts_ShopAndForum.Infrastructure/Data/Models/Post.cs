using AutoParts_ShopAndForum.Infrastructure.Data.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParts_ShopAndForum.Infrastructure.Data.Models
{
    public class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(PostContants.TitleMaxLength)]
        public string Title { get; set; }

        [Required]
        [MaxLength(PostContants.ContentMaxLength)]
        public string Content { get; set; }

        [Required]
        [ForeignKey(nameof(Creator))]
        public string CreatorId { get; set; }
        public virtual User Creator { get; set; }

        [ForeignKey(nameof(Category))]
        public int PostCategoryId { get; set; }
        public virtual PostCategory Category { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
