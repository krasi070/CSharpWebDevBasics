namespace TheSimpleWebServer.Server.Http
{
    using Common;

    public class HttpHeader
    {
        private string _key;
        private string _value;

        public HttpHeader(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public string Key
        {
            get
            {
                return this._key;
            }
            private set
            {
                CommonValidator.ThrowExceptionIfNullOrEmpty(value, nameof(value));
                this._key = value;
            }
        }

        public string Value
        {
            get
            {
                return this._value;
            }
            private set
            {
                CommonValidator.ThrowExceptionIfNullOrEmpty(value, nameof(value));
                this._value = value;
            }
        }

        public override string ToString() => $"{this.Key}: {this.Value}";
    }
}