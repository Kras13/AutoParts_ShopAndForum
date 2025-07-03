namespace AutoParts_ShopAndForum.Core.Models.Order;

public class OrderModel
{
    public int Id { get; set; }
        
    public Guid PublicToken { get; set; }

    public decimal OverallSum { get; set; }
    
    public DateTime DateCreated { get; set; }

    public DateTime DateDelivered { get; set; }

    public bool IsDelivered { get; set; }

    public DeliveryMethod DeliveryMethod { get; set; }

    public OrderPayWay PayWay { get; set; }

    public OnlinePaymentStatus? PaymentStatus { get; set; }
}