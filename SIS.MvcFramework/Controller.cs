namespace SIS.MvcFramework
{
    using System.IO;
    using System.Runtime.CompilerServices;
    using HTTP;
    using HTTP.Responses;

    public abstract class Controller
    {
        protected HttpResponse View<T>(T viewModel = null, 
            [CallerMemberName] string viewName = null)
            where T : class
        {
            IViewEngine viewEngine = new ViewEngine();

            var controllerName = this.GetType().Name.Replace("Controller", string.Empty);

            var templateHtml = File.ReadAllText("Views/" + controllerName + "/" + viewName + ".html");
            var html = viewEngine.GetHtml(templateHtml, viewModel);

            var layout = File.ReadAllText("Views/Shared/_Layout.html");
            var page = layout.Replace("@RenderBody()", html);

            page = viewEngine.GetHtml(page, viewModel);

            return new HtmlResponse(page);
        }

        protected HttpResponse View([CallerMemberName] string viewName = null)
        {
            return this.View<object>(null, viewName);
        }
    }
}
