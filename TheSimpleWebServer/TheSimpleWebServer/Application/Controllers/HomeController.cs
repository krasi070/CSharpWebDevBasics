namespace TheSimpleWebServer.Application.Controllers
{
    using Server.Enums;
    using Server.Http.Contracts;
    using Server.Http.Response;
    using Views.Home;

    public class HomeController
    {
        public IHttpResponse Index()
        {
            return new ViewResponse(HttpStatusCode.Ok, new IndexView());
        }
    }
}