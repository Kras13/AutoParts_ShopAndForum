using System.ComponentModel.DataAnnotations;
using AutoParts_ShopAndForum.Core.Models.Cart;
using AutoParts_ShopAndForum.Core.Models.CourierStation;
using AutoParts_ShopAndForum.Core.Models.Town;

namespace AutoParts_ShopAndForum.Models.Checkout;

public class CheckoutFormModel
{
    [Required]
    public DeliveryMethod DeliveryMethod { get; set; }

    [Required]
    [Display(Name = "����")]
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

    public IList<TownModel> Towns { get; set; }

    public IList<CourierStationModel> CourierStations { get; set; }
    public IList<ProductCartModel> Products { get; set; }
}