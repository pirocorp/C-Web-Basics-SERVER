namespace SIS.MvcFramework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using HTTP;
    using HTTP.Logging;
    using HTTP.Responses;

    public static class WebHost
    {
        public static async Task StartAsync(IMvcApplication application)
        {
            IList<Route> routeTable = new List<Route>();
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.Add<ILogger, ConsoleLogger>();

            application.ConfigureServices(serviceCollection);
            application.Configure(routeTable);

            var loggerInstance = serviceCollection.CreateInstance<ILogger>();

            AutoRegisterStaticFilesRoutes(routeTable);
            AutoRegisterActionRoutes(routeTable, application, 
                serviceCollection, loggerInstance);

            loggerInstance.Log($"Registered routes:");

            foreach (var route in routeTable)
            {
                loggerInstance.Log(route.ToString());
            }

            loggerInstance.Log();

            var httpServer = new HttpServer(80, routeTable, loggerInstance);
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

        private static void AutoRegisterStaticFilesRoutes(IList<Route> routeTable)
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
        /// <param name="serviceCollection">Dependency container</param>
        private static void AutoRegisterActionRoutes(IList<Route> routeTable,
            IMvcApplication application, IServiceCollection serviceCollection,
            ILogger logger)
        {
            var types = application
                .GetType()
                .Assembly
                .GetTypes()
                .Where(type => type.IsSubclassOf(typeof(Controller)) && !type.IsAbstract)
                .ToArray();

            logger.Log("Registered Controllers:");

            foreach (var controllerType in types)
            {
                logger.Log(controllerType.FullName);

                var methods = controllerType
                    .GetMethods()
                    .Where(m => 
                        !m.IsSpecialName 
                        && !m.IsConstructor
                        && m.IsPublic
                        && m.DeclaringType == controllerType
                        && m.GetBaseDefinition().DeclaringType == controllerType);

                foreach (var actionInfo in methods)
                {
                    var url = $"/{controllerType.Name.Replace("Controller", string.Empty)}/{actionInfo.Name}";

                    var attribute = actionInfo
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

                    //Action enclose this parameter
                    var controllerInstance = serviceCollection
                        .CreateInstance(controllerType) as Controller;

                    //Local function
                    HttpResponse Action(HttpRequest request)
                    {
                        controllerInstance.Request = request;

                        var parameterValues = new List<object>();

                        foreach (var parameter in actionInfo.GetParameters())
                        {
                            object parameterValue = request
                                .QueryData
                                .FirstOrDefault(x => x.Key.ToLower() == parameter.Name.ToLower())
                                .Value;

                            if (parameterValue == null)
                            {
                                parameterValue = request
                                    .FormData
                                    .FirstOrDefault(x => x.Key.ToLower() == parameter.Name.ToLower())
                                    .Value;
                            }

                            parameterValues.Add(Convert.ChangeType(parameterValue, parameter.ParameterType));
                        }

                        var response = actionInfo.Invoke(controllerInstance,
                            parameterValues.ToArray()) as HttpResponse;

                        return response;
                    }

                    //Local function is passed as parameter
                    var route = new Route(httpActionType, url, Action);
                    routeTable.Add(route);
                }
            }
        }
    }
}
