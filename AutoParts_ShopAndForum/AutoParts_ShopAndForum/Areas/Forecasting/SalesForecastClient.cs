namespace AutoParts_ShopAndForum.Areas.Forecasting;

public class SalesForecastClient : ISalesForecastClient
{
    public int? PredictMonthAsync(int productId, int year, int month)
    {
        throw new NotImplementedException();
    }

    public Dictionary<int, int> PredictYearlyAsync(int productId, int year)
    {
        throw new NotImplementedException();
    }
}