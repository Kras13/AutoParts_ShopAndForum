namespace AutoParts_ShopAndForum.Areas.Forecasting;

public interface ISalesForecastClient
{
    int? PredictMonthAsync(int productId, int year, int month);
    Dictionary<int, int> PredictYearlyAsync(int productId, int year);
}