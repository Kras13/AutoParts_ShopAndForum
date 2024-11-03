using AutoParts_ShopAndForum.Infrastructure.Data.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParts_ShopAndForum.Infrastructure.Data.Models
{
    public class ProductSubcategory
    {
        public ProductSubcategory()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(ProductSubcategoryConstants.NameMaxLength)]
        public string Name { get; set; }

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        public virtual ProductCategory Category { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
