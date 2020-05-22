namespace SIS.HTTP
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class HttpServer : IHttpServer
    {
        private readonly TcpListener _tcpListener;

        //TODO: actions
        public HttpServer(int port)
        {
            this._tcpListener = new TcpListener(IPAddress.Loopback, port);
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
            string requestString;
            var networkStream = tcpClient.GetStream();

            try
            {
                using (networkStream)
                {
                    //1MB TODO: Use buffer 4KB
                    var requestBytes = new byte[1_048_576];
                    var bytesRead = await networkStream.ReadAsync(requestBytes, 0, requestBytes.Length);
                    requestString = Encoding.UTF8.GetString(requestBytes, 0, bytesRead);

                    var request = new HttpRequest(requestString);

                    var content = "<h1>Random page</h1>";

                    if (request.Path == "/")
                    {
                        content = $"<h1>Hello World</h1>" + $"<h2>{DateTime.Now}</h2>" + 
                                  $"<form method='POST'><input name='username' /><input type='submit' /></form>";
                    }
                    else if(request.Path == "/users/login")
                    {
                        content = $"<h1>Login Page</h1>";
                    }

                    var contentBytes = Encoding.UTF8.GetBytes(content);
                    var response = new HttpResponse(HttpResponseCode.Ok, contentBytes);

                    response.Headers.Add(new Header("Server", "SIServer/0.01"));
                    response.Headers.Add(new Header("Content-Type", "text/html"));

                    response.Cookies.Add(
                        new ResponseCookie("sid", Guid.NewGuid().ToString())
                        {
                            HttpOnly = true,
                            MaxAge = 3600,
                        });

                    var responseBytes = Encoding.UTF8.GetBytes(response.ToString());

                    await networkStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    await networkStream.WriteAsync(response.Body, 0, response.Body.Length);
                }

                Console.WriteLine(requestString);
                Console.WriteLine(new string('=', Console.WindowWidth));
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