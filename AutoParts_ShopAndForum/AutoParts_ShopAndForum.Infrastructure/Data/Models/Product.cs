using AutoParts_ShopAndForum.Infrastructure.Data.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParts_ShopAndForum.Infrastructure.Data.Models
{
    public class Product
    {
        public Product()
        {
            ProductOrders = new HashSet<OrderProduct>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(ProductConstants.NameMaxLength)]
        public string Name { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public decimal Price { get; set; }

        [Required]
        [MaxLength(ProductConstants.DescriptionMaxLength)]
        public string Description { get; set; }

        [ForeignKey(nameof(Subcategory))]
        public int SubcategoryId { get; set; }

        public virtual ProductSubcategory Subcategory { get; set; }

        [Required]
        [ForeignKey(nameof(Creator))]
        public string CreatorId { get; set; }

        public virtual User Creator { get; set; }

        public virtual ICollection<OrderProduct> ProductOrders { get; set; }
    }
}
