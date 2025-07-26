using AutoParts_ShopAndForum.Areas.Seller.Models;

namespace AutoParts_ShopAndForum.Areas.Forecasting;

public interface ISalesForecastClient
{
    Task<int?>  PredictMonthAsync(int productId, int year, int month); 
    Task<List<MonthlyForecastResult>> PredictYearlyAsync(int productId, int year);
}