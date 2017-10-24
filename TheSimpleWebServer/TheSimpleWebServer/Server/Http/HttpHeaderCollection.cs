namespace TheSimpleWebServer.Server.Http
{
    using Common;
    using Contracts;
    using System;
    using System.Collections.Generic;

    public class HttpHeaderCollection : IHttpHeaderCollection
    {
        private readonly IDictionary<string, HttpHeader> _headers;

        public HttpHeaderCollection()
        {
            this._headers = new Dictionary<string, HttpHeader>();
        }

        public void Add(HttpHeader header)
        {
            CommonValidator.ThrowExceptionIfNull(header, nameof(header));
            _headers[header.Key] = header;
        }

        public bool ContainsKey(string key)
        {
            CommonValidator.ThrowExceptionIfNull(key, nameof(key));

            return this._headers.ContainsKey(key);
        }

        public HttpHeader Get(string key)
        {
            CommonValidator.ThrowExceptionIfNull(key, nameof(key));

            if (!this._headers.ContainsKey(key))
            {
                throw new InvalidOperationException($"The given key {key} is not present in the headers collection.");
            }

            return this._headers[key];
        }

        public override string ToString() => string.Join(Environment.NewLine, this._headers);
    }
}