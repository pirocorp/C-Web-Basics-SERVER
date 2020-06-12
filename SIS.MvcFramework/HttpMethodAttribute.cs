namespace SIS.MvcFramework
{
    using System;
    using HTTP;

    public abstract class HttpMethodAttribute : Attribute
    {
        protected HttpMethodAttribute()
        { }

        protected HttpMethodAttribute(string url)
        {
            this.Url = url;
        }

        public string Url { get; protected set; }

        public abstract HttpMethodType Type { get; }
    }
}
