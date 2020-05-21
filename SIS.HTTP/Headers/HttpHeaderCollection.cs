namespace SIS.HTTP.Headers
{
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Contracts;

    /// <summary>
    /// Repository like class
    /// </summary>
    public class HttpHeaderCollection : IHttpHeaderCollection
    {
        private readonly Dictionary<string, HttpHeader> _httpHeaders;

        public HttpHeaderCollection()
        {
            this._httpHeaders = new Dictionary<string, HttpHeader>();
        }

        public void AddHeader(HttpHeader header)
        {
            CoreValidator.ThrowIfNull(header, nameof(header));

            this._httpHeaders.Add(header.Key, header);
        }

        public bool ContainsHeader(string key)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));

            return this._httpHeaders.ContainsKey(key);
        }

        public HttpHeader GetHeader(string key)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));

            return this._httpHeaders[key];
        }

        public override string ToString() 
            => string.Join(GlobalConstants.HttpNewLine,
            this._httpHeaders.Values.Select(h => h.ToString()));
    }
}
