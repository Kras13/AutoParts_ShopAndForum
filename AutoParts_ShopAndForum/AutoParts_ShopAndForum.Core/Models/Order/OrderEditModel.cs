namespace AutoParts_ShopAndForum.Core.Models.Order;

public class OrderEditModel
{
    public int OrderId { get; set; }
    public bool IsDelivered { get; set; }
    public DateTime? DateDelivered { get; set; }
}