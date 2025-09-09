namespace AutoParts_ShopAndForum.Core.Models.EmailNotification
{
    public class OrderEmailModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemEmailModel> Items { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
