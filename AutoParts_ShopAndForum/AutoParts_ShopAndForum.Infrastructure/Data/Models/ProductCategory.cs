using AutoParts_ShopAndForum.Infrastructure.Data.Constants;
using System.ComponentModel.DataAnnotations;

namespace AutoParts_ShopAndForum.Infrastructure.Data.Models
{
    public class ProductCategory
    {
        public ProductCategory()
        {
            Subcategories = new HashSet<ProductSubcategory>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(ProductCategoryConstants.NameMaxLength)]
        public string Name { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public virtual ICollection<ProductSubcategory> Subcategories { get; set; }
    }
}
