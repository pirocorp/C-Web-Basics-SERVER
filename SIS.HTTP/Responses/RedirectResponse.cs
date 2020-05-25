namespace SIS.HTTP.Responses
{
    public class RedirectResponse : HttpResponse
    {
        public RedirectResponse(string newLocation)
        {
            this.Headers.Add(new Header("Location", newLocation));
            this.StatusCode = HttpResponseCode.Found;
        }
    }
}
