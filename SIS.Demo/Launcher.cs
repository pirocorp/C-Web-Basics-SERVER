namespace SIS.Demo
{
    using System;
    using HTTP.Enums;
    using WebServer;
    using WebServer.Routing;

    public static class Launcher
    {
        public static void Main()
        {
            var serverRoutingTable = new ServerRoutingTable();

            serverRoutingTable.Add(
                HttpRequestMethod.Get,
                path: "/",
                request => new HomeController().Index(request));

            var server = new Server(8000, serverRoutingTable);

            server.Run();
        }
    }
}
