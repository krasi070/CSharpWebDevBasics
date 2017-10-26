namespace TheSimpleWebServer.Server.Handlers
{
    using Common;
    using Contracts;
    using Http;
    using Http.Contracts;
    using System;

    public abstract class RequestHandler : IRequestHandler
    {
        private readonly Func<IHttpRequest, IHttpResponse> _handlingFunc;

        protected RequestHandler(Func<IHttpRequest, IHttpResponse> handlingFunc)
        {
            CommonValidator.ThrowExceptionIfNull(handlingFunc, nameof(handlingFunc));
            this._handlingFunc = handlingFunc;
        }

        public IHttpResponse Handle(IHttpContext context)
        {
            IHttpResponse response = this._handlingFunc(context.Request);
            response.Headers.Add(new HttpHeader("Content-Type", "text/plain"));

            return response;
        }
    }
}