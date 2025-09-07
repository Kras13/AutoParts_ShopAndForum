namespace AutoParts_ShopAndForum.Infrastructure.Options;

public class SmtpOptions
{
    public const string Section = "Smtp";

    public string Host { get; set; }

    public int Port { get; set; }
}