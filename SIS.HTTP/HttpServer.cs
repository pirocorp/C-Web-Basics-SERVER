namespace SIS.HTTP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class HttpServer : IHttpServer
    {
        private readonly TcpListener _tcpListener;
        private readonly IList<Route> _routingTable;

        //TODO: actions
        public HttpServer(int port, IList<Route> routingTable)
        {
            this._tcpListener = new TcpListener(IPAddress.Loopback, port);
            this._routingTable = routingTable;
        }

        public async Task StartAsync()
        {
            this._tcpListener.Start();

            while (true)
            {
                var tcpClient = await this._tcpListener.AcceptTcpClientAsync();
                #pragma warning disable 4014
                Task.Run(() => this.ProcessClientAsync(tcpClient));
                #pragma warning restore 4014
            }
        }

        public async Task ResetAsync()
        {
            this.Stop();
            await this.StartAsync();
        }

        public void Stop()
        {
            this._tcpListener.Stop();
        }

        private async Task ProcessClientAsync(TcpClient tcpClient)
        {
            var networkStream = tcpClient.GetStream();

            using (networkStream)
            {
                try
                {
                    //1MB TODO: Use buffer 4KB
                    var requestBytes = new byte[1_048_576];
                    var bytesRead = await networkStream.ReadAsync(requestBytes, 0, requestBytes.Length);
                    var requestString = Encoding.UTF8.GetString(requestBytes, 0, bytesRead);

                    var request = new HttpRequest(requestString);

                    Console.WriteLine($"{request.Method} {request.Path}");
                    Console.WriteLine(new string('=', Console.WindowWidth));

                    var route = this._routingTable
                        .FirstOrDefault(r => r.HttpMethod == request.Method
                                             && r.Path == request.Path);

                    var response = new HttpResponse(HttpResponseCode.NotFound, new byte[0]);

                    if (route != null)
                    {
                        response = route.Action(request);
                    }

                    response.Headers.Add(new Header("Server", "SIServer/0.01"));
                    response.Cookies.Add(
                    new ResponseCookie("sid", Guid.NewGuid().ToString())
                    {
                        HttpOnly = true,
                        MaxAge = 3600,
                    });

                    var responseBytes = Encoding.UTF8.GetBytes(response.ToString());

                    await networkStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    await networkStream.WriteAsync(response.Body, 0, response.Body.Length);
                    await networkStream.FlushAsync();
                }
                catch (Exception e)
                {
                    var errorResponse = new HttpResponse(
                        HttpResponseCode.InternalServerError,
                        Encoding.UTF8.GetBytes(e.ToString()));

                    errorResponse.Headers.Add(new Header("Content-Type", "text/plain"));

                    var errorResponseBytes = Encoding.UTF8.GetBytes(errorResponse.ToString());

                    await networkStream.WriteAsync(errorResponseBytes, 0, errorResponseBytes.Length);
                    await networkStream.WriteAsync(errorResponse.Body, 0, errorResponse.Body.Length);
                }
            }
        }
    }
}