using System.Text.Json;
using AutoParts_ShopAndForum.Areas.Seller.Models;

namespace AutoParts_ShopAndForum.Areas.Forecasting;

public class SalesForecastClient : ISalesForecastClient
{
    private readonly HttpClient _httpClient;

    public SalesForecastClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<int?> PredictMonthAsync(int productId, int year, int month)
    {
        var request = new
        {
            product_id = productId,
            year,
            month
        };

        var response = await _httpClient.PostAsJsonAsync("/predict", request);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        return json.GetProperty("predicted_quantity").GetInt32();
    }

    public async Task<List<MonthlyForecastResult>> PredictYearlyAsync(int productId, int year)
    {
        var request = new
        {
            product_id = productId,
            year
        };

        var response = await _httpClient.PostAsJsonAsync("/predict/yearly", request);

        if (!response.IsSuccessStatusCode)
            return [];

        var result = await response.Content.ReadFromJsonAsync<YearlyForecastResponse>();

        return result?.Forecast ?? [];
    }

}