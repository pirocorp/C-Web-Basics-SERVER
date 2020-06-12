namespace SIS.MvcFramework
{
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;
    using HTTP;
    using HTTP.Responses;

    public abstract class Controller
    {
        public HttpRequest Request { get; set; }

        protected HttpResponse View<T>(T viewModel = null, 
            [CallerMemberName] string viewName = null)
            where T : class
        {
            var controllerName = this.GetType().Name.Replace("Controller", string.Empty);
            var viewPath = "Views/" + controllerName + "/" + viewName + ".html";

            return this.ViewByName<T>(viewPath, viewModel);
        }

        protected HttpResponse View([CallerMemberName] string viewName = null)
        {
            return this.View<object>(null, viewName);
        }

        protected HttpResponse Error(string error)
        {
            var errorModel = new ErrorViewModel
            {
                Error = error,
            };

            return this.ViewByName<ErrorViewModel>("Views/Shared/Error.html", errorModel);
        }

        protected HttpResponse Redirect(string url)
        {
            return new RedirectResponse(url);
        }

        protected string Hash(string input)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(input));
            
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2")); //255 => FF
            }

            return hash.ToString();
        }

        private HttpResponse ViewByName<T>(string viewPath, object viewModel)
        {
            IViewEngine viewEngine = new ViewEngine();

            var templateHtml = File.ReadAllText(viewPath);
            var html = viewEngine.GetHtml(templateHtml, viewModel);

            var layout = File.ReadAllText("Views/Shared/_Layout.html");
            var page = layout.Replace("@RenderBody()", html);

            page = viewEngine.GetHtml(page, viewModel);

            return new HtmlResponse(page);
        }
    }
}
