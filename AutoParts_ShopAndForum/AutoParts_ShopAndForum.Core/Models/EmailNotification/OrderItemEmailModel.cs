namespace AutoParts_ShopAndForum.Core.Models.EmailNotification
{
    public class OrderItemEmailModel
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
