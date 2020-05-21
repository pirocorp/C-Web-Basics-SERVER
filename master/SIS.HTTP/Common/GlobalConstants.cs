namespace SIS.HTTP.Common
{
    public class GlobalConstants
    {
        public const string HttpOneProtocolFragment = "HTTP/1.1";

        public const string HostHeaderKey = "Host";
        
        /// <summary>
        /// CRLF token is \r\n
        /// </summary>
        public const string HttpNewLine = "\r\n";

        public const string UnsupportedHttpMethodExceptionMessage = "The method {0} is not suported.";
    }
}
