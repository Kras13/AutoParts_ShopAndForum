using AutoParts_ShopAndForum.Core.Models.Order;

namespace AutoParts_ShopAndForum.Areas.Administrator.Models.Order;

public class OrderSortingViewModel
{
    public string DisplayText { get; set; }
    public OrdersSorting Sorting { get; set; }
}