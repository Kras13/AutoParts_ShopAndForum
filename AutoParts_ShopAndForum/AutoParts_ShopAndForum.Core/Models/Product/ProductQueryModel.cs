namespace AutoParts_ShopAndForum.Core.Models.Product
{
    public class ProductQueryModel
    {
        public ProductModel[] Products { get; set; }
        public int TotalProductsWithoutPagination { get; set; }
    }
}
