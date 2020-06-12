namespace SIS.MvcFramework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
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

            AutoRegisterStaticFilesRoutes(routeTable);
            AutoRegisterActionRoutes(routeTable, application);

            Console.WriteLine($"Registered routes:");

            foreach (var route in routeTable)
            {
                Console.WriteLine(route);
            }

            Console.WriteLine();

            var httpServer = new HttpServer(80, routeTable);
            await httpServer.StartAsync();
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

        private static void AutoRegisterStaticFilesRoutes(List<Route> routeTable)
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
        
        /// <summary>
        /// Auto register action routes by convention
        /// /{controller}/{action}
        /// </summary>
        /// <param name="routeTable">Route table</param>
        /// <param name="application">Application context</param>
        private static void AutoRegisterActionRoutes(List<Route> routeTable,
            IMvcApplication application)
        {
            var types = application
                .GetType()
                .Assembly
                .GetTypes()
                .Where(type => type.IsSubclassOf(typeof(Controller)) && !type.IsAbstract)
                .ToArray();

            foreach (var type in types)
            {
                Console.WriteLine(type.FullName);

                var methods = type
                    .GetMethods()
                    .Where(m => 
                        !m.IsSpecialName 
                        && !m.IsConstructor
                        && m.IsPublic
                        && m.DeclaringType == type
                        && m.GetBaseDefinition().DeclaringType == type);

                foreach (var methodInfo in methods)
                {
                    var url = $"/{type.Name.Replace("Controller", string.Empty)}/{methodInfo.Name}";

                    var attribute = methodInfo
                        .GetCustomAttributes()
                        .FirstOrDefault(a => a.GetType().IsSubclassOf(typeof(HttpMethodAttribute)))
                        as HttpMethodAttribute;

                    var httpActionType = HttpMethodType.Get;

                    if (attribute != null)
                    {
                        httpActionType = attribute.Type;

                        if (attribute.Url != null)
                        {
                            url = attribute.Url;
                        }
                    }

                    var controllerInstance = Activator.CreateInstance(type) as Controller;

                    HttpResponse Action(HttpRequest request)
                    {
                        controllerInstance.Request = request;
                        return methodInfo.Invoke(controllerInstance, new object[]{}) as HttpResponse;
                    }

                    var route = new Route(httpActionType, url, Action);
                    routeTable.Add(route);
                }
            }
        }
    }
}
