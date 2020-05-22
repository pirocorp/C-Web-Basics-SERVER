namespace SIS.HTTP
{
    using System.Collections.Generic;
    using System.Text;

    public class HttpResponse
    {
        public HttpResponse(HttpResponseCode statusCode, byte[] body)
        {
            this.Headers = new List<Header>();
            this.Cookies = new List<ResponseCookie>();
            
            this.Version = HttpVersionType.Http11;
            this.StatusCode = statusCode;
            this.Body = body;

            this.Headers.Add(new Header("Content-Length", $"{body?.Length ?? 0}"));
        }

        public HttpVersionType Version { get; }

        public HttpResponseCode StatusCode { get; }

        public IList<Header> Headers { get; }

        public IList<ResponseCookie> Cookies { get; }

        public byte[] Body { get; }

        public override string ToString()
        {
            var response = new StringBuilder();

            var versionAsString = this.Version switch
            {
                HttpVersionType.Http10 => "HTTP/1.0",
                HttpVersionType.Http11 => "HTTP/1.1",
                HttpVersionType.Http20 => "HTTP/2.0",
                _ => "HTTP/1.1",
            };

            response.Append($"{versionAsString} {(int)this.StatusCode} {this.StatusCode.ToString()}" + HttpConstants.NewLine);

            foreach (var header in this.Headers)
            {
                response.Append(header + HttpConstants.NewLine);
            }

            foreach (var cookie in this.Cookies)
            {
                response.Append($"Set-Cookie: {cookie}" + HttpConstants.NewLine);
            }

            response.Append(HttpConstants.NewLine);
            return response.ToString();
        }
    }
}
