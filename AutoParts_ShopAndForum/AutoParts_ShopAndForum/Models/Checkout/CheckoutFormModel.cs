using System.ComponentModel.DataAnnotations;
using AutoParts_ShopAndForum.Attributes;
using AutoParts_ShopAndForum.Core.Models.Cart;
using AutoParts_ShopAndForum.Core.Models.CourierStation;
using AutoParts_ShopAndForum.Core.Models.Order;
using AutoParts_ShopAndForum.Core.Models.Town;
using AutoParts_ShopAndForum.Localization;
using AutoParts_ShopAndForum.Models.Cart;

namespace AutoParts_ShopAndForum.Models.Checkout;

public class CheckoutFormModel
{
    [Required(ErrorMessageResourceName = "InputFieldRequired", ErrorMessageResourceType = typeof(MainLocalization))]
    [Display(Name = "Метод на доставка")]
    public DeliveryMethod DeliveryMethod { get; set; }

    [Display(Name = "Адрес")]
    [Required(ErrorMessageResourceName = "InputFieldRequired", ErrorMessageResourceType = typeof(MainLocalization))]
    [CheckoutDeliveryAddress(DeliveryMethodName = nameof(DeliveryMethod))]
    public string DeliveryStreet { get; set; }
    
    [Display(Name = "Населено място")]
    public int DeliverToAddressTownId { get; set; }

    [Required(ErrorMessageResourceName = "InputFieldRequired", ErrorMessageResourceType = typeof(MainLocalization))]
    [Display(Name = "Населено място")]
    public int SelectedTownId { get; set; }

    [Required(ErrorMessageResourceName = "InputFieldRequired", ErrorMessageResourceType = typeof(MainLocalization))]
    [Display(Name = "Офис/автомат")]
    public int? SelectedCourierStationId { get; set; }

    [Required(ErrorMessageResourceName = "InputFieldRequired", ErrorMessageResourceType = typeof(MainLocalization))]
    [Display(Name = "Телефонен номер")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessageResourceName = "InputFieldRequired", ErrorMessageResourceType = typeof(MainLocalization))]
    [Display(Name = "Име")]
    public string InvoiceFirstName { get; set; }

    [Required(ErrorMessageResourceName = "InputFieldRequired", ErrorMessageResourceType = typeof(MainLocalization))]
    [Display(Name = "Фамилия")]
    public string InvoiceLastName { get; set; }

    [Required(ErrorMessageResourceName = "InputFieldRequired", ErrorMessageResourceType = typeof(MainLocalization))]
    [Display(Name = "Адрес")]
    public string InvoiceAddress { get; set; }

    [Required(ErrorMessageResourceName = "InputFieldRequired", ErrorMessageResourceType = typeof(MainLocalization))]
    public OrderPayWay PayWay { get; set; }

    public IList<TownModel> Towns { get; set; }
    public ICollection<ProductCartModel> Products { get; set; }
    public bool ReadOnlyMode { get; set; }
}