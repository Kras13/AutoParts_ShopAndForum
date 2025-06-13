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
    [Display(Name = "Town")]
    public int SelectedTownId { get; set; }

    [Required]
    [Display(Name = "Courier station")]
    public int SelectedCourierStationId { get; set; }

    public IList<TownModel> Towns { get; set; }

    public IList<CourierStationModel> CourierStations { get; set; }
    public ICollection<ProductCartModel> Products { get; set; }
}