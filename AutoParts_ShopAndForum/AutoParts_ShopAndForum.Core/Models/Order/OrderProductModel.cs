namespace AutoParts_ShopAndForum.Core.Models.Order;

public class OrderProductModel
{
    public int Id { get; set; }
    public decimal SinglePrice { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}