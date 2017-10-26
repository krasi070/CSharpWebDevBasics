namespace TheSimpleWebServer.Server.Http
{
    using Common;
    using Contracts;
    using Enums;
    using Exceptions;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Net;

    public class HttpRequest : IHttpRequest
    {
        public HttpRequest(string request)
        {
            this.FormData = new Dictionary<string, string>();
            this.QueryParameters = new Dictionary<string, string>();
            this.UrlParameters = new Dictionary<string, string>();
            this.Headers = new HttpHeaderCollection();
            this.ParseRequest(request);
        }

        public IDictionary<string, string> FormData { get; private set; }

        public HttpHeaderCollection Headers { get; private set; }

        public string Path { get; private set; }

        public IDictionary<string, string> QueryParameters { get; private set; }

        public HttpRequestMethod Method { get; private set; }

        public string Url { get; private set; }

        public IDictionary<string, string> UrlParameters { get; private set; }

        public void AddUrlParameters(string key, string value)
        {
            CommonValidator.ThrowExceptionIfNullOrEmpty(key, nameof(key));
            CommonValidator.ThrowExceptionIfNullOrEmpty(value, nameof(value));

            this.UrlParameters[key] = value;
        }

        private void ParseRequest(string requestText)
        {
            CommonValidator.ThrowExceptionIfNullOrEmpty(requestText, nameof(requestText));
            
            string[] requestLines = requestText.Split(Environment.NewLine);

            if (!requestLines.Any())
            {
                throw new BadRequestException();
            }

            var requestLine = requestLines[0]
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (requestLine.Length != 3 || requestLine[2].ToLower() != "http/1.1")
            {
                throw new BadRequestException();
            }

            this.Method = this.ParseMethod(requestLine[0]);
            this.Url = requestLine[1];
            this.Path = this.ParsePath(this.Url);
            this.ParseHeaders(requestLines);
            this.ParseParameters();
            this.ParseFormData(requestLines.Last());
        }

        private HttpRequestMethod ParseMethod(string method)
        {
            try
            {
                return Enum.Parse<HttpRequestMethod>(method, true);
            }
            catch (Exception)
            {

                throw new BadRequestException();
            }
        }

        private string ParsePath(string url)
        {
            return url.Split(new[] { '?', '#' }, StringSplitOptions.RemoveEmptyEntries)[0];
        }

        private void ParseHeaders(string[] requestLines)
        {
            var emptyLineIndex = Array.IndexOf(requestLines, string.Empty);
            for (int i = 1; i < emptyLineIndex; i++)
            {
                string currLine = requestLines[i];
                string[] headerParts = currLine.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                string headerKey = headerParts[0];
                string headerValue = headerParts[1].Trim();

                HttpHeader header = new HttpHeader(headerKey, headerValue);
                this.Headers.Add(header);
            }

            if (!this.Headers.ContainsKey("Host"))
            {
                throw new BadRequestException();
            }
        }

        private void ParseParameters()
        {
            if (!this.Url.Contains('?'))
            {
                return;
            }

            string query = this.Url
                .Split(new[] { '?' }, StringSplitOptions.RemoveEmptyEntries)
                .Last();

            this.ParseQuery(query, this.UrlParameters);
        }

        private void ParseFormData(string requestLine)
        {
            if (this.Method == HttpRequestMethod.Get)
            {
                return;
            }

            this.ParseQuery(requestLine, this.FormData);
        }

        private void ParseQuery(string queryString, IDictionary<string, string> dictionary)
        {
            if (!queryString.Contains('='))
            {
                return;
            }

            string[] queryPairs = queryString.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var queryPair in queryPairs)
            {
                string[] queryKeyValuePair = queryPair.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (queryKeyValuePair.Length != 2)
                {
                    throw new BadRequestException();
                }

                string queryKey = WebUtility.UrlDecode(queryKeyValuePair[0]);
                string queryValue = WebUtility.UrlDecode(queryKeyValuePair[1]);
                dictionary.Add(queryKey, queryValue);
            }
        }
    }
}