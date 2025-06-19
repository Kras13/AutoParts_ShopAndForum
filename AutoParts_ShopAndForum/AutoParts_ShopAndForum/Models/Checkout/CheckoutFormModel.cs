using System.ComponentModel.DataAnnotations;
using AutoParts_ShopAndForum.Core.Models.Cart;
using AutoParts_ShopAndForum.Core.Models.CourierStation;
using AutoParts_ShopAndForum.Core.Models.Order;
using AutoParts_ShopAndForum.Core.Models.Town;

namespace AutoParts_ShopAndForum.Models.Checkout;

public class CheckoutFormModel
{
    [Required]
    public DeliveryMethod DeliveryMethod { get; set; }

    [Display(Name = "�����")]
    public string DeliverAddress { get; set; }

    [Display(Name = "�������� �����")]
    public int DeliverToAddressTownId { get; set; }

    [Required]
    [Display(Name = "�������� �����")]
    public int SelectedTownId { get; set; }

    [Required]
    [Display(Name = "���� �������")]
    public int SelectedCourierStationId { get; set; }

    [Required]
    [Display(Name = "���")]
    public string InvoiceFirstName { get; set; }

    [Required]
    [Display(Name = "�������")]
    public string InvoiceLastName { get; set; }

    [Required]
    [Display(Name = "�����")]
    public string InvoiceAddress { get; set; }

    [Required]
    public OrderPayWay PayWay { get; set; }

    public IList<TownModel> Towns { get; set; }

    public IList<CourierStationModel> CourierStations { get; set; }
    public ICollection<ProductCartModel> Products { get; set; }
}