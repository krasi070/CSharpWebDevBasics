namespace TheSimpleWebServer.Server.Http.Response
{
    using Contracts;
    using Enums;
    using System.Text;

    public abstract class HttpResponse : IHttpResponse
    {
        protected HttpResponse()
        {
            this.Headers = new HttpHeaderCollection();
        }

        public HttpHeaderCollection Headers { get; }

        public HttpStatusCode StatusCode { get; protected set; }

        public string StatusCodeMessage => this.StatusCode.ToString();

        public override string ToString()
        {
            var response = new StringBuilder();
            int statusCodeNumber = (int)this.StatusCode;
            response.AppendLine($"HTTP/1.1 {statusCodeNumber} {this.StatusCodeMessage}");
            response.AppendLine(this.Headers.ToString());
            response.AppendLine();

            return response.ToString();
        }
    }
}