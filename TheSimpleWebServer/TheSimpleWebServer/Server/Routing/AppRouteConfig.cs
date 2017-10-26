namespace TheSimpleWebServer.Server.Routing
{
    using Contracts;
    using Enums;
    using Handlers;
    using Http.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AppRouteConfig : IAppRouteConfig
    {
        private readonly Dictionary<HttpRequestMethod, Dictionary<string, RequestHandler>> _routes;

        public AppRouteConfig()
        {
            this._routes = new Dictionary<HttpRequestMethod, Dictionary<string, RequestHandler>>();
            this.InitRoutes();
        }

        public IReadOnlyDictionary<HttpRequestMethod, Dictionary<string, RequestHandler>> Routes 
            => this._routes;

        public void Get(string route, Func<IHttpRequest, IHttpResponse> handler)
        {
            this.AddRoute(route, new GetHandler(handler));
        }

        public void Post(string route, Func<IHttpRequest, IHttpResponse> handler)
        {
            this.AddRoute(route, new PostHandler(handler));
        }

        public void AddRoute(string route, RequestHandler handler)
        {
            string handlerName = handler.GetType().ToString().ToLower();
            if (handlerName.Contains("get"))
            {
                this._routes[HttpRequestMethod.Get].Add(route, handler);
            }
            else if (handlerName.Contains("post"))
            {
                this._routes[HttpRequestMethod.Post].Add(route, handler);
            }
            else
            {
                throw new InvalidOperationException("Invalid handler!");
            }
        }

        private void InitRoutes()
        {
            var availableMethods = Enum
                .GetValues(typeof(HttpRequestMethod))
                .Cast<HttpRequestMethod>();

            foreach (var method in availableMethods)
            {
                this._routes[method] = new Dictionary<string, RequestHandler>();
            }
        }
    }
}