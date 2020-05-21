namespace SIS.HTTP.Exceptions
{
    using System;

    public class BadRequestException : Exception
    {
        private const string BAD_REQUEST_EXCEPTION_DEFAULT_MESSAGE =
            "The Request was malformed or contains unsupported elements.";

        public BadRequestException()
            : this(BAD_REQUEST_EXCEPTION_DEFAULT_MESSAGE)
        {
        }

        public BadRequestException(string message)
            : base(message)
        {
            
        }
    }
}
