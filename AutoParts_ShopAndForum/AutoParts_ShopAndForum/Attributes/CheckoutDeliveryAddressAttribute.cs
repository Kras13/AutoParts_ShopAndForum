using System.ComponentModel.DataAnnotations;
using AutoParts_ShopAndForum.Core.Models.Order;

namespace AutoParts_ShopAndForum.Attributes;

public class CheckoutDeliveryAddressAttribute : ValidationAttribute
{
    public string DeliveryMethodName { get; set; }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var deliveryMethod = validationContext.Items[DeliveryMethodName] as DeliveryMethod?;

        if (deliveryMethod is DeliveryMethod.PersonalTake)
            return ValidationResult.Success;

        if (value is null)
            return new ValidationResult("Delivery address must be provided.");
        
        return ValidationResult.Success;
    }
}