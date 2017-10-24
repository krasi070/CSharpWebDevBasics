namespace TheSimpleWebServer.Server.Common
{
    using System;

    public class CommonValidator
    {
        public static void ThrowExceptionIfNull(object obj, string name)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void ThrowExceptionIfNullOrEmpty(string text, string name)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(name, $"{name} cannot be null or empty!");
            }
        }
    }
}