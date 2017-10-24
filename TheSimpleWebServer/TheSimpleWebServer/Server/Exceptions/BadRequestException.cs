namespace TheSimpleWebServer.Server.Exceptions
{
    using System;

    public class BadRequestException : Exception
    {
        private const string InvalidRequestMessage = "Request is not valid!";

        public BadRequestException()
            : base(InvalidRequestMessage)
        {

        }
    }
}