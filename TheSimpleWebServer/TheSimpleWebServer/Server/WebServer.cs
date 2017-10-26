namespace TheSimpleWebServer.Server
{
    using Contracts;
    using Routing;
    using Routing.Contracts;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class WebServer : IRunnable
    {
        private const string LocalHostIpAddress = "127.0.0.1";

        private readonly int _port;
        private readonly IServerRouteConfig _serverRouteConfig;
        private readonly TcpListener _listener;

        private bool _isRunning; 

        public WebServer(int port, IAppRouteConfig routeConfig)
        {
            this._port = port;
            this._listener = new TcpListener(IPAddress.Parse(LocalHostIpAddress), port);
            this._serverRouteConfig = new ServerRouteConfig(routeConfig);
        }

        public void Run()
        {
            this._listener.Start();
            this._isRunning = true;

            Console.WriteLine($"Server running on {LocalHostIpAddress}:{this._port}");
            Task.Run(this.ListenLoop).Wait();
        }

        private async Task ListenLoop()
        {
            while (this._isRunning)
            {
                var client = await this._listener.AcceptSocketAsync();
                var connectionHandler = new ConnectionHandler(client, this._serverRouteConfig);
                await connectionHandler.ProcessRequestAsync();
            }
        }
    }
}