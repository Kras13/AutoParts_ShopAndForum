using AutoParts_ShopAndForum.Core.Models.Product;

namespace AutoParts_ShopAndForum.Core.Models.Order;

public class OrderDetailsModel
{
    public int Id { get; set; }
    public ICollection<OrderProductModel> Products { get; set; }
    public decimal OverallSum { get; set; }
}