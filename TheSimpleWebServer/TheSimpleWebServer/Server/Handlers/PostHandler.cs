namespace TheSimpleWebServer.Server.Handlers
{
    using System;
    using Http.Contracts;

    public class PostHandler :RequestHandler
    {
        public PostHandler(Func<IHttpRequest, IHttpResponse> handlingFunc)
            : base(handlingFunc)
        {

        }
    }
}