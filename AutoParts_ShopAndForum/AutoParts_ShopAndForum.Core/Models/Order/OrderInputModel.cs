namespace AutoParts_ShopAndForum.Core.Models.Order;

public class OrderInputModel
{
    public DeliveryMethod DeliveryMethod { get; set; }
    public OrderPayWay PayWay { get; set; }
    public string DeliveryStreet { get; set; }
    public int TownId { get; set; }
    public int? CourierStationId { get; set; }
    public string InvoicePersonFirstName { get; set; }
    public string InvoicePersonLastName { get; set; }
    public string InvoiceAddress { get; set; }
    public string UserId { get; set; }
}