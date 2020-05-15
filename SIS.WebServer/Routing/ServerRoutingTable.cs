namespace SIS.WebServer.Routing
{
    using System;
    using System.Collections.Generic;

    using Contracts;
    using HTTP.Common;
    using HTTP.Enums;
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;

    public class ServerRoutingTable : IServerRoutingTable
    {
        private Dictionary<HttpRequestMethod, Dictionary<string, Func<IHttpRequest, IHttpResponse>>> _routingTable;

        public ServerRoutingTable()
        {
            this._routingTable = new Dictionary<HttpRequestMethod, Dictionary<string, Func<IHttpRequest, IHttpResponse>>>
            {
                [HttpRequestMethod.Get] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>(),
                [HttpRequestMethod.Post] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>(),
                [HttpRequestMethod.Put] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>(),
                [HttpRequestMethod.Delete] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>(),
            };
        }

        public void Add(HttpRequestMethod method, string path, Func<IHttpRequest, IHttpResponse> func)
        {
            CoreValidator.ThrowIfNull(method, nameof(method));
            CoreValidator.ThrowIfNullOrEmpty(path, nameof(path));
            CoreValidator.ThrowIfNull(func, nameof(func));

            this._routingTable[method].Add(path, func);
        }

        public bool Contains(HttpRequestMethod method, string path)
        {
            CoreValidator.ThrowIfNull(method, nameof(method));
            CoreValidator.ThrowIfNullOrEmpty(path, nameof(path));

            return this._routingTable.ContainsKey(method)
                   && this._routingTable[method].ContainsKey(path);
        }

        public Func<IHttpRequest, IHttpResponse> Get(HttpRequestMethod method, string path)
        {
            CoreValidator.ThrowIfNull(method, nameof(method));
            CoreValidator.ThrowIfNullOrEmpty(path, nameof(path));

            return this._routingTable[method][path];
        }
    }
}
