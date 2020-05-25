namespace DemoApp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using SIS.HTTP;
    using SIS.HTTP.Responses;

    public static class DemoAppProgram
    {
        public static async Task Main()
        {
            var db = new ApplicationDbContext();

            var routeTable = new List<Route>();

            routeTable.Add(new Route(HttpMethodType.Get, "/", Index));
            routeTable.Add(new Route(HttpMethodType.Post, "/Tweets/Create", CreateTweet));
            routeTable.Add(new Route(HttpMethodType.Get, "/favicon.ico", FavIcon));

            var httpServer = new HttpServer(80, routeTable);
            await httpServer.StartAsync();
        }

        //TODO Action Headers to return all request headers as HTML

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

            var db = new ApplicationDbContext();

            var tweets = db.Tweets
                .Select(t => new
                {
                    t.CreatedOn,
                    t.Creator,
                    t.Content
                })
                .ToList();

            var html = new StringBuilder();

            html.Append("<table><tr><th>Date</th><th>Creator</th><th>Content</th></tr>");

            foreach (var tweet in tweets)
            {
                html.Append($"<tr><td>{tweet.CreatedOn}</td><td>{tweet.Creator}</td><td>{tweet.Content}</td></tr>");
            }

            html.Append("</table>");
            html.Append(
                "<form action='/Tweets/Create' method='post'><input type='text' name='creator' /><br /><textarea name='tweetText'></textarea><br /><input type='submit' value='Submit' /></form>");

            return new HtmlResponse(html.ToString());
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

        private static HttpResponse CreateTweet(HttpRequest request)
        {
            var db = new ApplicationDbContext();
            db.Tweets.Add(new Tweet
            {
                CreatedOn = DateTime.UtcNow,
                Creator = request.FormData["creator"],
                Content = request.FormData["tweetText"],
            });

            db.SaveChanges();

            return new RedirectResponse("/");
        }
    }
}
