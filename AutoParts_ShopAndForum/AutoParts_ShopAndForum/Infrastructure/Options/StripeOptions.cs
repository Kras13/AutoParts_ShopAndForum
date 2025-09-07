namespace AutoParts_ShopAndForum.Infrastructure.Options;

public class StripeOptions
{
    public const string Section = "Stripe";

    public string SecretKey { get; set; }

    public string WebhookSecret { get; set; }
}