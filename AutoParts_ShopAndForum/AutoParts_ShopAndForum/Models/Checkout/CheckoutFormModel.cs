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
    [Display(Name = "Град")]
    public int SelectedTownId { get; set; }

    [Required]
    [Display(Name = "Офис станция")]
    public int SelectedCourierStationId { get; set; }

    [Required]
    [Display(Name = "Име")]
    public string InvoiceFirstName { get; set; }

    [Required]
    [Display(Name = "Фамилия")]
    public string InvoiceLastName { get; set; }

    public IList<TownModel> Towns { get; set; }

    public IList<CourierStationModel> CourierStations { get; set; }
    public IList<ProductCartModel> Products { get; set; }
}