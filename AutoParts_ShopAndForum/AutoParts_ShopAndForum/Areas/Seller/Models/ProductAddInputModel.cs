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
        [MinLength(2, ErrorMessageResourceName = "InputFieldAtLeastLength", ErrorMessageResourceType = typeof(MainLocalization))]
        [Display(Name = "ProductAdd_Name", ResourceType = typeof(MainLocalization))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = "InputFieldRequired", ErrorMessageResourceType = typeof(MainLocalization))]
        [MinLength(2, ErrorMessageResourceName = "InputFieldAtLeastLength", ErrorMessageResourceType = typeof(MainLocalization))]
        [Display(Name = "ProductAdd_ImageUrl", ResourceType = typeof(MainLocalization))]
        public string ImageUrl { get; set; }

        [Required(ErrorMessageResourceName = "InputFieldRequired", ErrorMessageResourceType = typeof(MainLocalization))]
        [Display(Name = "ProductAdd_Price", ResourceType = typeof(MainLocalization))]
        [Range(0.01, 10000, ErrorMessageResourceName = "ProductAdd_PriceRange", ErrorMessageResourceType = typeof(MainLocalization))]
        public decimal Price { get; set; }
        
        [Required(ErrorMessageResourceName = "InputFieldRequired", ErrorMessageResourceType = typeof(MainLocalization))]
        [Display(Name = "ProductAdd_Description", ResourceType = typeof(MainLocalization))]
        [MaxLength(ProductConstants.DescriptionMaxLength)]
        [MinLength(2, ErrorMessageResourceName = "InputFieldAtLeastLength", ErrorMessageResourceType = typeof(MainLocalization))]
        public string Description { get; set; }

        public ProductSubcategoryModel[] Subcategories { get; set; }

        [Required(ErrorMessageResourceName = "InputFieldRequired", ErrorMessageResourceType = typeof(MainLocalization))]
        [Display(Name = "ProductAdd_SelectedSubcategory", ResourceType = typeof(MainLocalization))]
        public int SelectedSubcategoryId { get; set; }

        public string CreatorId { get; set; }
    }
}
