namespace SIS.MvcFramework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using HTTP;
    using HTTP.Responses;

    public static class WebHost
    {
        public static async Task StartAsync(IMvcApplication application)
        {
            var routeTable = new List<Route>();

            application.ConfigureServices();
            application.Configure(routeTable);

            AutoRegisterRoutes(routeTable, application);

            foreach (var route in routeTable)
            {
                Console.WriteLine(route);
            }

            var httpServer = new HttpServer(80, routeTable);
            await httpServer.StartAsync();
        }

        private static void AutoRegisterRoutes(List<Route> routeTable, 
            IMvcApplication application)
        {
            var files = Directory.GetFiles("./wwwroot", "*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fileContent = File.ReadAllBytes(file);
                var fileExtension = GetFileExtension(file);
                var contentType = GetContentType(fileExtension);
                var fileUrl = GetFileUrl(file);

                routeTable.Add(new Route(HttpMethodType.Get, fileUrl, 
                    (request) => new FileResponse(fileContent, contentType)));
            }
        }

        private static string GetFileExtension(string file)
            => new FileInfo(file)
                .Extension
                .Substring(1);

        private static string GetContentType(string fileExtension)
            => fileExtension switch
            {
                "css"  => "text/css",
                "html"  => "text/html",
                "js"   => "text/javascript",
                "ico"  => "image/x-icon",
                "jpg"  => "image/jpeg",
                "jpeg" => "image/jpeg",
                "png"  => "image/png",
                "gif"  => "image/gif",
                _      => "text/plain"
            };

        private static string GetFileUrl(string file)
            => file
                .Replace("./wwwroot", string.Empty)
                .Replace(@"\", "/");
    }
}
