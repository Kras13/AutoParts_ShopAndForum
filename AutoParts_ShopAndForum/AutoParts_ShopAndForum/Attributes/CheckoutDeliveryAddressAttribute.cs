using System.ComponentModel.DataAnnotations;
using AutoParts_ShopAndForum.Core.Models.Order;
using AutoParts_ShopAndForum.Models.Checkout;

namespace AutoParts_ShopAndForum.Attributes;

public class CheckoutDeliveryAddressAttribute : ValidationAttribute
{
    public string DeliveryMethodName { get; set; }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var formModel = (CheckoutFormModel)validationContext.ObjectInstance;
        var deliveryMethod = formModel.DeliveryMethod;

        if (deliveryMethod is DeliveryMethod.PersonalTake)
            return ValidationResult.Success;

        if (value is null)
            return new ValidationResult("Delivery address must be provided.");
        
        return ValidationResult.Success;
    }
}