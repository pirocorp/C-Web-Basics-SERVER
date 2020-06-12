namespace SIS.MvcFramework
{
    using System.IO;
    using System.Runtime.CompilerServices;
    using HTTP;
    using HTTP.Responses;

    public abstract class Controller
    {
        protected HttpResponse View([CallerMemberName] string viewName = null)
        {
            IViewEngine viewEngine = new ViewEngine();

            var controllerName = this.GetType().Name.Replace("Controller", string.Empty);

            var templateHtml = File.ReadAllText("Views/" + controllerName + "/" + viewName + ".html");
            var html = viewEngine.GetHtml(templateHtml, null);

            var layout = File.ReadAllText("Views/Shared/_Layout.html");
            var page = layout.Replace("@RenderBody()", html);

            page = viewEngine.GetHtml(page, null);

            return new HtmlResponse(page);
        }
    }
}
