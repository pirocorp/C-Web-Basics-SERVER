namespace SIS.HTTP.Responses
{
    using System.Text;

    public class HtmlResponse : HttpResponse
    {
        public HtmlResponse(string html)
        {
            this.StatusCode = HttpResponseCode.Ok;

            var byteData = Encoding.UTF8.GetBytes(html);
            this.Body = byteData;

            this.Headers.Add(new Header("Content-Type", "text/html"));
            this.Headers.Add(new Header("Content-Length", $"{this.Body?.Length ?? 0}"));
        }
    }
}
