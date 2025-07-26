using AutoParts_ShopAndForum.Areas.Forecasting;
using AutoParts_ShopAndForum.Areas.Seller.Models;
using AutoParts_ShopAndForum.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoParts_ShopAndForum.Areas.Seller.Controllers;

public class ForecastController : BaseSellerController
{
    private readonly ISalesForecastClient _forecastClient;
    private readonly IProductService _productService;

    public ForecastController(ISalesForecastClient forecastClient, IProductService productService)
    {
        _forecastClient = forecastClient;
        _productService = productService;
    }

    public IActionResult Index()
    {
        var products = _productService.GetAll();
        
        var viewModel = new ForecastViewModel()
        {
            SelectedYear = DateTime.Now.Year,
            Products = products.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).ToList()
        };
        
        return View(viewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> Index(ForecastViewModel model)
    {
        var products = _productService.GetAll();
            
        model.Products = products.Select(p => new SelectListItem
        {
            Text = p.Name,
            Value = p.Id.ToString()
        }).ToList();

        if (!ModelState.IsValid)
            return View(model);
        
        var forecast = await _forecastClient.PredictYearlyAsync(model.SelectedProductId, model.SelectedYear);
            
        model.ForecastResults = forecast;

        return View(model);
    }
}