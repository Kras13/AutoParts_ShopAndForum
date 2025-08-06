using AutoParts_ShopAndForum.Core.Models.Product;
using AutoParts_ShopAndForum.Core.Services;
using AutoParts_ShopAndForum.Localization;
using AutoParts_ShopAndForum.Models.ProductSubcategory;

namespace AutoParts_ShopAndForum.Models.Product
{
    public class ProductQueryViewModel
    {
        public int CurrentPage { get; set; } = 1;

        public PageSizeViewModel[] Pages { get; set; } =
        [
            new() { SizeDisplayText = "2", Size = 2 },
            new() { SizeDisplayText = "3", Size = 3 },
            new() { SizeDisplayText = "10", Size = 10 },
            new() { SizeDisplayText = MainLocalization.ProductQueryViewModel_AllPages, Size = ProductService.AllProducts }
        ];

        public int ProductsPerPage { get; set; } = 2;
        public int TotalProducts { get; set; }
        public string SearchCriteria { get; set; }
        public ProductSorting Sorting { get; set; }

        public ProductSortingViewModel[] Sortings { get; set; } =
        [
            new() { DisplayText = MainLocalization.ProductQueryViewModel_NoSorting, Sorting = ProductSorting.NoSorting },
            new() { DisplayText = MainLocalization.ProductQueryViewModel_PriceAscending, Sorting = ProductSorting.PriceAscending },
            new() { DisplayText = MainLocalization.ProductQueryViewModel_PriceDescending, Sorting = ProductSorting.PriceDescending },
            new() { DisplayText = MainLocalization.ProductQueryViewModel_NameAscending, Sorting = ProductSorting.NameAscending },
            new() { DisplayText = MainLocalization.ProductQueryViewModel_NameDescending, Sorting = ProductSorting.NameDescending },
            new() { DisplayText = MainLocalization.ProductQueryViewModel_DateAddedAscending, Sorting = ProductSorting.DateAscenging },
            new() { DisplayText = MainLocalization.ProductQueryViewModel_DateAddedDescending, Sorting = ProductSorting.DateDescending }
        ];

        public int? CategoryId { get; set; }
        public string CurrentUrl { get; set; }
        public ProductModel[] Products { get; set; }
        public ProductSubcategorySelectViewModel[] Subcategories { get; set; }
    }
}