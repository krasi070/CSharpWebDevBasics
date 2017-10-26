namespace TheSimpleWebServer.Application
{
    using Controllers;
    using Server.Contracts;
    using Server.Routing.Contracts;

    class MainApplication : IApplication
    {
        public void Configure(IAppRouteConfig appRouteConfig)
        {
            appRouteConfig.Get("/", request => new HomeController().Index());
        }
    }
}