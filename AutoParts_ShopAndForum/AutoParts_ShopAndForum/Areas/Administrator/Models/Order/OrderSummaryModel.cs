using AutoParts_ShopAndForum.Core.Models.Order;
using AutoParts_ShopAndForum.Localization;

namespace AutoParts_ShopAndForum.Areas.Administrator.Models.Order;

public class OrderSummaryModel
{
    public int OrderId { get; set; }
    public string Username { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateDelivered { get; set; }
    public OnlinePaymentStatus? OnlinePaymentStatus { get; set; }
    
    public OrderStatus Status => DateDelivered.HasValue
        ? OrderStatus.Delivered
        : OrderStatus.Pending;

    public string StatusAsText => Status == OrderStatus.Delivered
        ? MainLocalization.OrderStatus_Delivered
        : MainLocalization.OrderStatus_Pending;
}