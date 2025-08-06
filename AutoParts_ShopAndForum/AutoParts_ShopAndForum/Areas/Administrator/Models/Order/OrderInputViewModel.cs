using AutoParts_ShopAndForum.Core.Models.Order;
using AutoParts_ShopAndForum.Models.Checkout;

namespace AutoParts_ShopAndForum.Areas.Administrator.Models.Order;

public class OrderInputViewModel
{
    public int OrderId { get; set; }
    public CheckoutFormModel ReadOnlyModel { get; set; }
    public bool IsDelivered { get; set; }
    public DateTime? DateDelivered { get; set; }
    
    // public int Id { get; set; }
    // public DateTime DateCreated { get; set; }
    // public DateTime? DateDelivered { get; set; }
    // public bool IsDelivered { get; set; }
    // public DeliveryMethod DeliveryMethod { get; set; }
    // public OrderPayWay PayWay { get; set; }
    // public OnlinePaymentStatus? OnlinePaymentStatus { get; set; }
    // public string DeliveryStreet { get; set; }
    // public int TownId { get; set; }
    // public int? CourierStationId { get; set; }
    // public string InvoicePersonFirstName { get; set; }
    // public string InvoicePersonLastName { get; set; }
    // public string InvoiceAddress { get; set; }
    // public string UserId { get; set; }
}