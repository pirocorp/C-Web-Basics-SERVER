namespace SIS.HTTP
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class HttpServer : IHttpServer
    {
        private string _newLine;
        private readonly TcpListener _tcpListener;

        //TODO: actions
        public HttpServer(int port)
        {
            this._newLine = Environment.NewLine;
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
            using (var networkStream = tcpClient.GetStream())
            {
                //1MB TODO: Use buffer 4KB
                var requestBytes = new byte[1_000_000];
                var bytesRead = await networkStream.ReadAsync(requestBytes, 0, requestBytes.Length);
                var requestString = Encoding.UTF8.GetString(requestBytes, 0, bytesRead);

                var responseText = $"<h1>Hello World</h1>" + $"<h2>{DateTime.Now}</h2>";

                var response = "HTTP/1.1 200 OK" + this._newLine +
                               "Server: PiroServer/1.0" + this._newLine +
                               "Content-Type: text/html" + this._newLine +
                               $"Set-Cookie: user=Piroman; Path=/; Expires={DateTime.UtcNow.AddMinutes(5):R}" + this._newLine + 
                               "Set-Cookie: lang=bg; Path=/lang" + this._newLine + 
                               //"Content-Disposition: attachment; filename=niki.html" download as file + newLine +
                               //"Location: https://softuni.bg" + newLine +
                               "Content-Length: " +responseText.Length + this._newLine +
                               this._newLine + 
                               responseText;

                var responseBytes = Encoding.UTF8.GetBytes(response);
                await networkStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                Console.WriteLine(requestString);
            }
            
            Console.WriteLine(new string('=', Console.WindowWidth));
        }
    }
}