namespace TheSimpleWebServer.Server.Routing
{
    using Contracts;
    using Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class ServerRouteConfig : IServerRouteConfig
    {
        private readonly IDictionary<HttpRequestMethod, IDictionary<string, IRoutingContext>> _routes;

        public ServerRouteConfig(IAppRouteConfig appRouteConfig)
        {
            this._routes = new Dictionary<HttpRequestMethod, IDictionary<string, IRoutingContext>>();
            this.InitRoutes();
            this.InitRouteConfig(appRouteConfig);
        }

        public IDictionary<HttpRequestMethod, IDictionary<string, IRoutingContext>> Routes => this._routes;

        private void InitRoutes()
        {
            var availableMethods = Enum
                .GetValues(typeof(HttpRequestMethod))
                .Cast<HttpRequestMethod>();

            foreach (var method in availableMethods)
            {
                this._routes[method] = new Dictionary<string, IRoutingContext>();
            }
        }

        private void InitRouteConfig(IAppRouteConfig appRouteConfig)
        {
            foreach (var registeredRoute in appRouteConfig.Routes)
            {
                var requestMethod = registeredRoute.Key;
                var routeWithHandlers = registeredRoute.Value;

                foreach (var routeWithHandler in routeWithHandlers)
                {
                    string route = routeWithHandler.Key;
                    var handler = routeWithHandler.Value;

                    var parameters = new List<string>();
                    var parsedRouteRegex = this.ParseRoute(route, parameters);
                    var routingContext = new RoutingContext(handler, parameters);
                    this._routes[requestMethod].Add(parsedRouteRegex, routingContext);
                }
            }
        }

        private string ParseRoute(string route, List<string> parameters)
        {
            var result = new StringBuilder();
            result.Append("^");

            if (route == "/")
            {
                result.Append("/$");

                return result.ToString();
            }

            var tokens = route.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            this.ParseTokens(tokens, parameters, result);

            return result.ToString();
        }

        private void ParseTokens(string[] tokens, List<string> parameters, StringBuilder result)
        {
            for (int i = 0; i < tokens.Length; i++)
            {
                var end = i == tokens.Length - 1 ? "$" : "/";
                var currToken = tokens[i];

                if (currToken.StartsWith("{") && currToken.EndsWith("}"))
                {
                    result.Append($"{currToken}{end}");
                    continue;
                }

                var parameterRegex = new Regex("<\\w+>");
                var parameterMatch = parameterRegex.Match(currToken);
                if (!parameterMatch.Success)
                {
                    throw new InvalidOperationException($"Route parameter in '{currToken}' is not valid!");
                }

                var match = parameterMatch.Value;
                var parameter = match.Substring(1, match.Length - 2);
                parameters.Add(parameter);

                var currTokenWithoutBrackets = currToken.Substring(1, currToken.Length - 2);
                result.Append($"{currTokenWithoutBrackets}{end}");
            }
        }
    }
}