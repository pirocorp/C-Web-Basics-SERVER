namespace DemoApp
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using SIS.HTTP;
    using SIS.HTTP.Responses;

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
            var byteContent = File.ReadAllBytes("wwwroot/favicon.ico");

            return new FileResponse(byteContent, "image/x-icon");
        }

        private static HttpResponse Contact(HttpRequest request)
        {
            return new HtmlResponse("<h1>Contact page</h1>");
        }

        private static HttpResponse Index(HttpRequest request)
        {
            var user = request.SessionData.ContainsKey("Username") 
                       ? request.SessionData["Username"]
                       : "Guest";
            return new HtmlResponse($"<h1>Home Page. Hello, {user}</h1><h2>TEST TEST</h2><img src='/images/img.jpeg' />");
        }

        private static HttpResponse Login(HttpRequest request)
        {
            request.SessionData["Username"] = "Piroman";
            return new HtmlResponse("<h1>Login page</h1>");
        }

        private static HttpResponse DoLogin(HttpRequest request)
        {
            return new HtmlResponse("<h1>Login page</h1>");
        }
    }
}
