namespace SIS.Demo.Controllers
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    using HTTP.Enums;
    using HTTP.Responses.Contracts;
    using WebServer.Results;

    public abstract class BaseController
    {
        public IHttpResponse View([CallerMemberName] string view = null)
        {
            var controllerName = this
                .GetType()
                .Name
                .Replace("Controller", string.Empty);

            Console.WriteLine($"Controller: {controllerName}, Method: {view}");

            var viewContent = File.ReadAllText("Views/" + controllerName + "/" + view + ".html");

            return new HtmlResult(viewContent, HttpResponseStatusCode.Ok);
        }
    }
}
