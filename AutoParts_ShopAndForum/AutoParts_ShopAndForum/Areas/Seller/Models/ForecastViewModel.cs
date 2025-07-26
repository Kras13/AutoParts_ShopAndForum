using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoParts_ShopAndForum.Areas.Seller.Models;

public class ForecastViewModel
{
    public int SelectedProductId { get; set; }
    public int SelectedYear { get; set; } = DateTime.Now.Year;

    public List<SelectListItem> Products { get; set; } = new();
    public List<MonthlyForecastResult> ForecastResults { get; set; } = new();
}