namespace SIS.HTTP
{
    using System;

    public class Route
    {
        public Route(HttpMethodType method, string path, Func<HttpRequest, HttpResponse> action)
        {
            this.Path = path;
            this.HttpMethod = method;
            this.Action = action;
        }

        public string Path { get; }

        public HttpMethodType HttpMethod { get; }

        public Func<HttpRequest, HttpResponse> Action { get; }
    }
}
