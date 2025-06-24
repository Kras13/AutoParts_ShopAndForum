using System.ComponentModel.DataAnnotations;
using AutoParts_ShopAndForum.Core.Models.Cart;
using AutoParts_ShopAndForum.Core.Models.CourierStation;
using AutoParts_ShopAndForum.Core.Models.Order;
using AutoParts_ShopAndForum.Core.Models.Town;
using AutoParts_ShopAndForum.Models.Cart;

namespace AutoParts_ShopAndForum.Models.Checkout;

public class CheckoutFormModel
{
    [Required]
    public DeliveryMethod DeliveryMethod { get; set; }

    [Display(Name = "Адрес")]
    public string DeliverAddress { get; set; }

    [Display(Name = "Населено място")]
    public int DeliverToAddressTownId { get; set; }

    [Required]
    [Display(Name = "Населено място")]
    public int SelectedTownId { get; set; }

    [Required]
    [Display(Name = "Офис/автомат")]
    public int SelectedCourierStationId { get; set; }

    [Required]
    [Display(Name = "Име")]
    public string InvoiceFirstName { get; set; }

    [Required]
    [Display(Name = "Фамилия")]
    public string InvoiceLastName { get; set; }

    [Required]
    [Display(Name = "Адрес")]
    public string InvoiceAddress { get; set; }

    [Required]
    public OrderPayWay PayWay { get; set; }

    public IList<TownModel> Towns { get; set; }

    public IList<CourierStationModel> CourierStations { get; set; }

    public CartSummaryModel Summary { get; set; }
    public ICollection<ProductCartModel> Products { get; set; }
}