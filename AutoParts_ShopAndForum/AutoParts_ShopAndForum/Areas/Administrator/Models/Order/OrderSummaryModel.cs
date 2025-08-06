using System.ComponentModel.DataAnnotations;
using AutoParts_ShopAndForum.Core.Models.Order;
using AutoParts_ShopAndForum.Localization;

namespace AutoParts_ShopAndForum.Areas.Administrator.Models.Order;

public class OrderSummaryModel
{
    [Display(Name = "OrderSummary_OrderId", ResourceType = typeof(MainLocalization))]
    public int OrderId { get; set; }
    
    [Display(Name = "OrderSummary_Username", ResourceType = typeof(MainLocalization))]
    public string Username { get; set; }
    
    [Display(Name = "OrderSummary_DateCreated", ResourceType = typeof(MainLocalization))]
    public DateTime DateCreated { get; set; }
    
    [Display(Name = "OrderSummary_DateDelivered", ResourceType = typeof(MainLocalization))]
    public DateTime? DateDelivered { get; set; }

    [Display(Name = "OrderSummary_PayWay", ResourceType = typeof(MainLocalization))]
    public OrderPayWay PayWay { get; set; }

    public string PayWayAsText => PayWay == OrderPayWay.OnlinePayment
        ? MainLocalization.OrderSummaryViewModel_OnlinePayWay
        : MainLocalization.OrderSummaryViewModel_CashOnDeliveryPayWay;

    [Display(Name = "OrderSummary_OrderStatus", ResourceType = typeof(MainLocalization))]
    public OrderStatus Status => DateDelivered.HasValue
        ? OrderStatus.Delivered
        : OrderStatus.Pending;

    public string StatusAsText => Status == OrderStatus.Delivered
        ? MainLocalization.OrderStatus_Delivered
        : MainLocalization.OrderStatus_Pending;

    [Display(Name = "OrderSummary_OverallSum", ResourceType = typeof(MainLocalization))]
    public decimal OverallSum { get; set; }
}