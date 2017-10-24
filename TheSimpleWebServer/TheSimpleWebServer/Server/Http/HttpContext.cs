namespace TheSimpleWebServer.Server.Http
{
    using Common;
    using Contracts;

    public class HttpContext : IHttpContext
    {
        private readonly IHttpRequest _request;

        public HttpContext(IHttpRequest request)
        {
            CommonValidator.ThrowExceptionIfNull(request, nameof(request));
            this._request = request;
        }

        public IHttpRequest Request => this._request;
    }
}