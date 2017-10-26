namespace TheSimpleWebServer.Server.Handlers
{
    using Common;
    using Contracts;
    using Http.Contracts;
    using Http.Response;
    using Routing.Contracts;
    using System.Text.RegularExpressions;
    

    public class HttpHandler : IRequestHandler
    {
        private readonly IServerRouteConfig _serverouteConfig;

        public HttpHandler(IServerRouteConfig routeConfig)
        {
            CommonValidator.ThrowExceptionIfNull(routeConfig, nameof(routeConfig));
            this._serverouteConfig = routeConfig;
        }

        public IHttpResponse Handle(IHttpContext context)
        {
            var requestMethod = context.Request.Method;
            var requestPath = context.Request.Path;
            var registerdRoutes = this._serverouteConfig.Routes[requestMethod];

            foreach (var regiseredRoute in registerdRoutes)
            {
                var routePattern = regiseredRoute.Key;
                var routingContext = regiseredRoute.Value;

                var routeRegex = new Regex(routePattern);
                var match = routeRegex.Match(requestPath);

                if (!match.Success)
                {
                    continue;
                }

                var parameters = routingContext.Parameters;
                foreach (var parameter in parameters)
                {
                    var parameterValue = match.Groups[parameter].Value;
                    context.Request.AddUrlParameters(parameter, parameterValue);
                }

                return routingContext.Handler.Handle(context);
            }

            return new NotFoundResponse();
        }
    }
}