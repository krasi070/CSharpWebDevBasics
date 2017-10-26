namespace TheSimpleWebServer.Server.Routing
{
    using Common;
    using Contracts;
    using Handlers;
    using System.Collections.Generic;

    public class RoutingContext : IRoutingContext
    {
        public RoutingContext(RequestHandler handler, IEnumerable<string> parameters)
        {
            CommonValidator.ThrowExceptionIfNull(handler, nameof(handler));
            CommonValidator.ThrowExceptionIfNull(parameters, nameof(parameters));
            this.Handler = handler;
            this.Parameters = parameters;
        }

        public IEnumerable<string> Parameters { get; private set; }

        public RequestHandler Handler { get; private set; }
    }
}