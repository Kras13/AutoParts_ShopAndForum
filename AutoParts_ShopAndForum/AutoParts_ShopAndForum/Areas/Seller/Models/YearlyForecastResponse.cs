namespace AutoParts_ShopAndForum.Areas.Seller.Models;

public class YearlyForecastResponse
{
    public int ProductId { get; set; }
    public List<MonthlyForecastResult> Forecast { get; set; } = [];
}
