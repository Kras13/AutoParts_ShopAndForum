namespace AutoParts_ShopAndForum.Core.Models.Order;

public class OrderQueryModel
{
    public OrderModel[] Orders { get; set; }
    public int TotalOrdersWithoutPagination { get; set; }
}