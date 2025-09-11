using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AutoParts_ShopAndForum.Service;

public class RazorViewToStringRenderer(
    IRazorViewEngine viewEngine,
    ITempDataProvider tempDataProvider,
    IActionContextAccessor actionContextAccessor,
    IServiceProvider serviceProvider)
{
    public async Task<string> RenderViewToStringAsync<T>(string viewPath, T model)
    {
        var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };
        var actionContext = actionContextAccessor.ActionContext;

        if (actionContext == null)
        {
            var routeData = new RouteData();

            actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());
        }

        var viewEngineResult = viewEngine.GetView(executingFilePath: null, viewPath: viewPath, isMainPage: true);

        if (!viewEngineResult.Success)
            throw new FileNotFoundException(
                $"View '{viewPath}' not found. Searched: {string.Join(", ", viewEngineResult.SearchedLocations)}");

        var view = viewEngineResult.View;

        using var result = new StringWriter();

        var viewContext = new ViewContext(
            actionContext,
            view,
            new ViewDataDictionary<T>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            },
            new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
            result,
            new HtmlHelperOptions()
        );

        await view.RenderAsync(viewContext);

        return result.ToString();
    }
}