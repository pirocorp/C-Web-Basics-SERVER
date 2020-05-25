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
            var layout = File.ReadAllText("Views/Shared/_Layout.html");

            var controllerName = this.GetType().Name.Replace("Controller", string.Empty);

            var html = File.ReadAllText("Views/" + controllerName + "/" + viewName + ".html");
            var page = layout.Replace("@RenderBody()", html);

            return new HtmlResponse(page);
        }
    }
}
