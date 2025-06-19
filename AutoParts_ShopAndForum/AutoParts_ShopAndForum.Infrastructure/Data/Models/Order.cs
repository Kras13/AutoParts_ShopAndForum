using AutoParts_ShopAndForum.Infrastructure.Data.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParts_ShopAndForum.Infrastructure.Data.Models
{
    public class Order
    {
        public Order()
        {
            OrderProducts = new HashSet<OrderProduct>();
        }

        public int Id { get; set; }

        public decimal OverallSum { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public DateTime DateDelivered { get; set; }

        public bool IsDelivered { get; set; }

        public DeliveryMethod DeliveryMethod { get; set; }

        [MaxLength(OrderConstants.StreetMaxLength)]
        public string DeliveryStreet { get; set; }

        [ForeignKey(nameof(Town))]
        public int TownId { get; set; }

        public virtual Town Town { get; set; }

        [ForeignKey(nameof(CourierStation))]
        public int? CourierStationId { get; set; }

        public CourierStation CourierStation { get; set; }

        [Required]
        [MaxLength(OrderConstants.InvoicePersonFirstNameMaxLength)]
        public string InvoicePersonFirstName { get; set; }

        [Required]
        [MaxLength(OrderConstants.InvoicePersonLastNameMaxLength)]
        public string InvoicePersonLastName { get; set; }

        [Required]
        [MaxLength(OrderConstants.InvoiceAddressMaxLength)]
        public string InvoiceAddress { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
