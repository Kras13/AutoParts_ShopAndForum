using AutoParts_ShopAndForum.Core.Models.ProductSubcategory;
using System.ComponentModel.DataAnnotations;
using AutoParts_ShopAndForum.Localization;

namespace AutoParts_ShopAndForum.Areas.Seller.Models
{
    public class ProductAddInputModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessageResourceName = "InputFieldRequired", ErrorMessageResourceType = typeof(MainLocalization))]
        [MaxLength(ProductConstants.NameMaxLength)]
        [MinLength(2, ErrorMessage = "Please enter at least 2 symbols")]
        [Display(Name = "ProductAdd_Name", ResourceType = typeof(MainLocalization))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = "InputFieldRequired", ErrorMessageResourceType = typeof(MainLocalization))]
        [MinLength(2, ErrorMessageResourceName = "InputFieldAtLeastLength", ErrorMessageResourceType = typeof(MainLocalization))]
        [Display(Name = "Image url")]
        public string ImageUrl { get; set; }

        [Required]
        //[Range(0.01, 10000, ErrorMessage = "Please enter value between 0.01 and 10000")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(ProductConstants.DescriptionMaxLength)]
        [MinLength(2, ErrorMessage = "Please enter at least 2 symbols")]
        public string Description { get; set; }

        public ProductSubcategoryModel[] Subcategories { get; set; }

        [Required]
        [Display(Name = "Subcategory")]
        public int SelectedSubcategoryId { get; set; }

        public string CreatorId { get; set; }
    }
}
