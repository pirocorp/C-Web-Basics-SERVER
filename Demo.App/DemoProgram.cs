namespace Demo.App
{
    using System;
    using System.Globalization;
    using System.Text;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Requests;
    using SIS.HTTP.Responses;

    public static class DemoProgram
    {
        public static void Main()
        {
            var request = "POST /url/asd?name=john&id=1#fragment HTTP/1.1\r\n"
                          + "Authorization: Basic\r\n"
                          + "Date: " + DateTime.Now + "\r\n"
                          + "Host: localhost:5000\r\n"
                          + "\r\n" 
                          + "username=johndoe&password=123";

            var httpRequest = new HttpRequest(request);

            var response = new HttpResponse(HttpResponseStatusCode.InternalServerError);
            response.AddHeader(new HttpHeader("Host", "localhost:5000"));
            response.AddHeader(new HttpHeader("Date", DateTime.Now.ToString(CultureInfo.InvariantCulture)));

            response.Content = Encoding.UTF8.GetBytes("<h1>Hello World!</h1>");

            Console.WriteLine(Encoding.UTF8.GetString(response.GetBytes()));
        }
    }
}
