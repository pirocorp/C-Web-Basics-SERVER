namespace SIS.HTTP.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Contracts;
    using Enums;
    using Exceptions;
    using Headers;
    using Headers.Contracts;

    public class HttpRequest : IHttpRequest
    {
        public HttpRequest(string requestString)
        {
            CoreValidator.ThrowIfNullOrEmpty(requestString, nameof(requestString));

            this.FormData = new Dictionary<string, object>();
            this.QueryData = new Dictionary<string, object>();
            this.Headers = new HttpHeaderCollection();

            this.ParseRequest(requestString);
        }

        public string Path { get; private set; }

        public string Url { get; private set; }

        public Dictionary<string, object> FormData { get; }

        public Dictionary<string, object> QueryData { get; }

        public IHttpHeaderCollection Headers { get; }

        public HttpRequestMethod RequestMethod { get; private set; }

        private bool IsValidRequestLine(string[] requestLineParams)
        {
            if (requestLineParams.Length != 3
                || requestLineParams[2] != GlobalConstants.HttpOneProtocolFragment)
            {
                return false;
            }

            return true;
        }

        private bool IsValidRequestQueryString(string queryString, string[] queryParameters)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<string> ParsePlainRequestHeaders(string[] requestLines)
        {
            for (var i = 1; i < requestLines.Length - 1; i++)
            {
                if(!string.IsNullOrEmpty(requestLines[i]))
                {
                    yield return requestLines[i];
                }
            }
        }

        private void ParseRequestMethod(string[] requestLineParams)
        {
            var isValid = Enum.TryParse<HttpRequestMethod>(requestLineParams[0], true, out var requestMethod);

            if (!isValid)
            {
                var errorMessage = string.Format(
                    GlobalConstants.UnsupportedHttpMethodExceptionMessage,
                    requestLineParams[0]);

                throw new BadRequestException(errorMessage);
            }

            this.RequestMethod = requestMethod;
        }

        private void ParseRequestUrl(string[] requestLineParams)
        {
            this.Url = requestLineParams[1];
        }

        private void ParseRequestPath()
        {
            this.Path = this.Url.Split('?')[0];
        }

        private void ParseRequestHeaders(string[] headersStrings)
        {
            headersStrings
                .Select(h => h.Split(": "))
                .Select(h => new HttpHeader(h[0], h[1]))
                .ToList()
                .ForEach(this.Headers.AddHeader);
        }

        private void ParseRequestQueryParameters()
        {
            // /users/profile?name="pesho"&id="asd"#fragment
            this.Url
                .Split('?', '#')[1]
                .Split('&')
                .Select(qp => qp.Split('='))
                .ToList()
                .ForEach(qp => this.QueryData.Add(qp[0], qp[1]));
        }

        private void ParseRequestFormDataParameters(string requestBody)
        {
            //TODO: Parse Multiple Parameters By Name
            requestBody
                .Split('&')
                .Select(qp => qp.Split('='))
                .ToList()
                .ForEach(qp => this.FormData.Add(qp[0], qp[1]));
        }

        private void ParseRequestParameters(string requestBody)
        {
            this.ParseRequestQueryParameters();
            this.ParseRequestFormDataParameters(requestBody); //TODO: Split
        }

        private void ParseRequest(string requestString)
        {
            var splitRequestString = requestString
                .Split(new[] {GlobalConstants.HttpNewLine}, StringSplitOptions.None);

            var requestLineParams = splitRequestString[0]
                .Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            if (!this.IsValidRequestLine(requestLineParams))
            {
                throw new BadRequestException();
            }

            this.ParseRequestMethod(requestLineParams);
            this.ParseRequestUrl(requestLineParams);
            this.ParseRequestPath();

            this.ParseRequestHeaders(this.ParsePlainRequestHeaders(splitRequestString).ToArray());
            //this.ParseCookies();
            this.ParseRequestParameters(splitRequestString[^1]);
        }
    }
}
