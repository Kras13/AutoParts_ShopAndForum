namespace AutoParts_ShopAndForum.Core.Models.Order;

public class OrderDetailsModel
{
    public int Id { get; set; }
    public ICollection<OrderProductModel> Products { get; set; }
    public decimal OverallSum { get; set; }
    public string InvoiceFirstName { get; set; }
    public string InvoiceLastName { get; set; }
    public string InvoiceAddress { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
    public string DeliveryStreet { get; set; }
    public string Town { get; set; }
    public string CourierStationAddress { get; set; }
    public DateTime? DateDelivered { get; set; }
    public OrderPayWay PayWay { get; set; }
    public OnlinePaymentStatus? OnlinePaymentStatus { get; set; }
    public int TownId { get; set; }
    public int? CourierStationId { get; set; }
    public bool IsDelivered { get; set; }
}