using AutoParts_ShopAndForum.Infrastructure.Data.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParts_ShopAndForum.Infrastructure.Data.Models
{
    public class Order
    {
        public Order()
        {
            Products = new HashSet<OrderProduct>();
        }

        public int Id { get; set; }

        public decimal OverallSum { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public DateTime DateDelivered { get; set; }

        public bool IsDelivered { get; set; }

        [Required]
        [MaxLength(OrderConstants.StreetMaxLength)]
        public string Street { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [ForeignKey(nameof(Town))]
        public int TownId { get; set; }

        public virtual Town Town { get; set; }

        public virtual ICollection<OrderProduct> Products { get; set; }
    }
}
