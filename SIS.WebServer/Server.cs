namespace SIS.WebServer
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using HTTP.Common;
    using Routing.Contracts;

    public class Server
    {
        private const string LOCAL_HOST_IP_ADDRESS = "127.0.0.1";

        private readonly int _port;

        private IServerRoutingTable _serverRoutingTable;

        private bool _isRunning;

        private readonly TcpListener _tcpListener;


        public Server(int port, IServerRoutingTable serverRoutingTable)
        {
            CoreValidator.ThrowIfNull(serverRoutingTable, nameof(serverRoutingTable));

            this._port = port;
            this._serverRoutingTable = serverRoutingTable;
            this._isRunning = false;

            this._tcpListener = new TcpListener(IPAddress.Parse(LOCAL_HOST_IP_ADDRESS), port);
        }

        public void Run()
        {
            this._tcpListener.Start();
            this._isRunning = true;

            Console.WriteLine($"Server started at http://{LOCAL_HOST_IP_ADDRESS}:{this._port}");

            while (this._isRunning)
            {
                Console.WriteLine("Waiting for client...");

                var client = this._tcpListener.AcceptSocket();

                this.Listen(client);
            }
        }

        public void Listen(Socket client)
        {
            var connectionHandler = new ConnectionHandler(client, this._serverRoutingTable);
            connectionHandler.ProcessRequest();
        }
    }
}
