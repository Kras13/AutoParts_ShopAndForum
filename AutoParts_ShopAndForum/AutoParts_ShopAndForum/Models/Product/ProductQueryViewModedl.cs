using AutoParts_ShopAndForum.Core.Models.Product;
using AutoParts_ShopAndForum.Core.Services;
using AutoParts_ShopAndForum.Models.ProductSubcategory;

namespace AutoParts_ShopAndForum.Models.Product
{
    public class ProductQueryViewModel
    {
        public ProductQueryViewModel()
        {
            Pages = new PageViewModel[]
            {
                new PageViewModel() {DisplayText = "2", PageIndex = 2},
                new PageViewModel() {DisplayText = "3", PageIndex = 3},
                new PageViewModel() {DisplayText = "10", PageIndex = 10},
                new PageViewModel() {DisplayText = "All", PageIndex = ProductService.AllProducts}
            };

            Sortings = new ProductSortingViewModel[]
            {
                new ProductSortingViewModel() {DisplayText = "No sorting", Sorting = ProductSorting.NoSorting},
                new ProductSortingViewModel() {DisplayText = "Price ascending", Sorting = ProductSorting.PriceAscending},
                new ProductSortingViewModel() {DisplayText = "Price descending", Sorting = ProductSorting.PriceDescending},
                new ProductSortingViewModel() {DisplayText = "Name ascending", Sorting = ProductSorting.NameAscending},
                new ProductSortingViewModel() {DisplayText = "Name descending", Sorting = ProductSorting.NameDescending},
                new ProductSortingViewModel() {DisplayText = "Date added ascending", Sorting = ProductSorting.DateAscenging},
                new ProductSortingViewModel() {DisplayText = "Date added descending", Sorting = ProductSorting.DateDescending},
            };
        }

        public int CurrentPage { get; set; } = 1;
        public PageViewModel[] Pages { get; set; }
        public int ProductsPerPage { get; set; }
        public int TotalProducts { get; set; }
        public string SearchCriteria { get; set; }
        public ProductSorting Sorting { get; set; }
        public ProductSortingViewModel[] Sortings { get; set; }
        public int? CategoryId { get; set; }
        public string CurrentUrl { get; set; }
        public ProductModel[] Products { get; set; }
        public ProductSubcategorySelectViewModel[] Subcategories { get; set; }
    }
}
