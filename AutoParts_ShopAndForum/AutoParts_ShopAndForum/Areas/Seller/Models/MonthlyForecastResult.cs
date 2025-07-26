using System.Text.Json.Serialization;

namespace AutoParts_ShopAndForum.Areas.Seller.Models;

public class MonthlyForecastResult
{
    public int Month { get; set; }
    
    public int Year { get; set; }
    
    [JsonPropertyName("predicted_quantity")]
    public int PredictedQuantity { get; set; }
}