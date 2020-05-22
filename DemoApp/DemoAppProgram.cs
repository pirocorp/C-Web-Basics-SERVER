namespace DemoApp
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using SIS.HTTP;

    public static class DemoAppProgram
    {
        public static async Task Main()
        {
            var routeTable = new List<Route>();

            routeTable.Add(new Route(HttpMethodType.Get, "/", Index));
            routeTable.Add(new Route(HttpMethodType.Get, "/users/login", Login));
            routeTable.Add(new Route(HttpMethodType.Post, "/users/login", DoLogin));
            routeTable.Add(new Route(HttpMethodType.Get, "/contact", Contact));
            routeTable.Add(new Route(HttpMethodType.Get, "/favicon.ico", FavIcon));

            var httpServer = new HttpServer(80, routeTable);
            await httpServer.StartAsync();
        }

        private static HttpResponse FavIcon(HttpRequest request)
        {
            throw new System.NotImplementedException();
        }

        private static HttpResponse Contact(HttpRequest request)
        {
            var content = "<h1>Contact page</h1>";
            var contentAsByteArray = Encoding.UTF8.GetBytes(content);

            var response = new HttpResponse(HttpResponseCode.Ok, contentAsByteArray);
            response.Headers.Add(new Header("Content-Type", "text/html"));
            return response;
        }

        public static HttpResponse Index(HttpRequest request)
        {
            var content = "<h1>Home page</h1>";
            var contentAsByteArray = Encoding.UTF8.GetBytes(content);

            var response = new HttpResponse(HttpResponseCode.Ok, contentAsByteArray);
            response.Headers.Add(new Header("Content-Type", "text/html"));
            return response;
        }

        public static HttpResponse Login(HttpRequest request)
        {
            var content = "<h1>Login page</h1>";
            var contentAsByteArray = Encoding.UTF8.GetBytes(content);

            var response = new HttpResponse(HttpResponseCode.Ok, contentAsByteArray);
            response.Headers.Add(new Header("Content-Type", "text/html"));
            return response;
        }

        public static HttpResponse DoLogin(HttpRequest request)
        {
            var content = "<h1>Login page</h1>";
            var contentAsByteArray = Encoding.UTF8.GetBytes(content);

            var response = new HttpResponse(HttpResponseCode.Ok, contentAsByteArray);
            response.Headers.Add(new Header("Content-Type", "text/html"));
            return response;
        }
    }
}
