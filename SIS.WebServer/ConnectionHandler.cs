namespace SIS.WebServer
{
    using System;
    using System.Net.Sockets;
    using System.Text;

    using HTTP.Common;
    using HTTP.Enums;
    using HTTP.Exceptions;
    using HTTP.Requests;
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;
    using Results;
    using Routing.Contracts;

    public class ConnectionHandler
    {
        private readonly Socket _client;

        private readonly IServerRoutingTable _serverRoutingTable;

        public ConnectionHandler(Socket client, IServerRoutingTable serverRoutingTable)
        {
            CoreValidator.ThrowIfNull(client, nameof(client));
            CoreValidator.ThrowIfNull(serverRoutingTable, nameof(serverRoutingTable));

            this._client = client;
            this._serverRoutingTable = serverRoutingTable;
        }

        public void ProcessRequest()
        {
            try
            {
                var httpRequest = this.ReadRequest();

                if (httpRequest != null)
                {
                    Console.WriteLine($"Processing: {httpRequest.RequestMethod} {httpRequest.Path}...");

                    var httpResponse = this.HandleRequest(httpRequest);

                    this.PrepareResponse(httpResponse);
                }
            }
            catch (BadRequestException e)
            {
                this.PrepareResponse(new TextResult(e.ToString(), HttpResponseStatusCode.BadRequest));
            }
            catch (Exception e)
            {
                this.PrepareResponse(new TextResult(e.ToString(), HttpResponseStatusCode.InternalServerError));
            }

            this._client.Shutdown(SocketShutdown.Both);
        }

        private IHttpRequest ReadRequest()
        {
            var result = new StringBuilder();
            var data = new ArraySegment<byte>(new byte[4096]); //4KB

            while (true)
            {
                var numberOfBytesRead = this._client.Receive(data.Array, SocketFlags.None);

                if (numberOfBytesRead == 0)
                {
                    break;
                }

                var byteAsString = Encoding.UTF8.GetString(data.Array, 0, numberOfBytesRead);
                result.Append(byteAsString);

                if (numberOfBytesRead < 4095)
                {
                    break;
                }
            }

            if (result.Length == 0)
            {
                return null;
            }

            return new HttpRequest(result.ToString());
        }

        private IHttpResponse HandleRequest(IHttpRequest request)
        {
            if (!this._serverRoutingTable.Contains(request.RequestMethod, request.Path))
            {
                return new TextResult(
                    $"Route with method {request.RequestMethod} and path \"{request.Path}\" not found.", 
                    HttpResponseStatusCode.NotFound);
            }

            return this._serverRoutingTable
                .Get(request.RequestMethod, request.Path)
                .Invoke(request);
        }

        private void PrepareResponse(IHttpResponse httpResponse)
        {
            var byteSegments = httpResponse.GetBytes();

            this._client.Send(byteSegments, SocketFlags.None);
        }
    }
}
