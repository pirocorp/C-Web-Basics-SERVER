namespace SIS.HTTP
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class HttpRequest
    {
        private readonly List<Header> _headers;
        private readonly List<Cookie> _cookies;

        public HttpRequest(string httpRequestAsString)
        {
            this._headers = new List<Header>();
            this._cookies = new List<Cookie>();

            var lines = httpRequestAsString.Split(
                new []{ HttpConstants.NewLine }, 
                StringSplitOptions.None);

            var httpInfoHeader = lines[0];
            var infoHeaderParts = httpInfoHeader.Split(' ');

            if (infoHeaderParts.Length != 3)
            {
                throw new HttpServerException("Invalid HTTP header line.");
            }

            var isParsed = Enum.TryParse<HttpMethodType>(infoHeaderParts[0], true, out var httpMethod);

            if (isParsed)
            {
                this.Method = httpMethod;
            }
            else
            {
                throw new HttpServerException("Unsupported HTTP method.");
            }

            this.Path = infoHeaderParts[1];

            var httpVersion = infoHeaderParts[2];

            this.Version = httpVersion switch
            {
                "HTTP/1.0" => HttpVersionType.Http10,
                "HTTP/1.1" => HttpVersionType.Http11,
                "HTTP/2.0" => HttpVersionType.Http20,
                _ => HttpVersionType.Http11, //Default value
            };

            var isInHeader = true;
            var bodyBuilder = new StringBuilder();
            for (var i = 1; i < lines.Length; i++)
            {
                var currentLine = lines[i];

                if (string.IsNullOrEmpty(currentLine))
                {
                    isInHeader = false;
                    continue;
                }

                if (isInHeader)
                {
                    var headerParts = currentLine
                        .Split(new[] {": "}, 
                            2, 
                            StringSplitOptions.None);

                    if (headerParts.Length != 2)
                    {
                        throw new HttpServerException($"Invalid header: {currentLine}");
                    }

                    if (headerParts[0] == "Cookie")
                    {
                        var cookiesAsString = headerParts[1];

                        var cookies = cookiesAsString
                            .Split(new string[] {"; "}, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var cookieString in cookies)
                        {
                            var cookieParts = cookieString
                                .Split(new[]{'='}, 2);

                            if (cookieParts.Length == 2)
                            {
                                this._cookies.Add(new Cookie(cookieParts[0], cookieParts[1]));
                            }
                        }
                    }

                    var header = new Header(headerParts[0], headerParts[1]);
                    this._headers.Add(header);
                }
                else
                {
                    bodyBuilder.AppendLine(currentLine);
                }
            }

            this.Body = bodyBuilder.ToString();
        }

        public HttpMethodType Method { get; }

        public string Path { get; }

        public HttpVersionType Version { get; }

        public IEnumerable<Header> Headers => this._headers.AsReadOnly();

        public IEnumerable<Cookie> Cookies => this._cookies.AsReadOnly(); 

        public string Body { get; }

        public IDictionary<string, string> SessionData { get; set; }
    }
}
