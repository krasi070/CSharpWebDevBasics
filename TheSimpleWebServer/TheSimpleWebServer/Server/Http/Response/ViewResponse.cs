namespace TheSimpleWebServer.Server.Http.Response
{
    using Enums;
    using Exceptions;
    using Server.Contracts;

    public class ViewResponse : HttpResponse
    {
        private readonly IView _view;

        public ViewResponse(HttpStatusCode statusCode, IView view)
        {
            this._view = view;
            this.StatusCode = statusCode;
        }

        public override string ToString()
        {
            return $"{base.ToString()}{this._view.View()}";
        }

        private void ValidateStatusCode(HttpStatusCode statusCode)
        {
            int statusCodeNumber = (int)this.StatusCode;
            if (statusCodeNumber >= 300 && statusCodeNumber < 400)
            {
                throw new InvalidResponseException("View responses need a status code above 300 and below 400!");
            }
        }
    }
}