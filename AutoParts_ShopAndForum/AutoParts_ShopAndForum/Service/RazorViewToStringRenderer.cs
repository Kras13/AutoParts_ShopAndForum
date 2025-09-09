using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AutoParts_ShopAndForum.Service
{
    public class RazorViewToStringRenderer(
        IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
    {
        private readonly IRazorViewEngine _viewEngine = viewEngine;
        private readonly ITempDataProvider _tempDataProvider = tempDataProvider;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task<string> RenderViewToStringAsync<T>(string viewPath, T model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            var viewEngineResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewPath, isMainPage: true);

            if (!viewEngineResult.Success)
                throw new FileNotFoundException($"View '{viewPath}' not found. Searched: {string.Join(", ", viewEngineResult.SearchedLocations ?? [])}");

            var view = viewEngineResult.View;

            using var result = new StringWriter();

            var viewContext = new ViewContext(
                actionContext,
                view,
                new ViewDataDictionary<T>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                },
                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                result,
                new HtmlHelperOptions()
            );

            await view.RenderAsync(viewContext);

            return result.ToString();
        }
    }
}
