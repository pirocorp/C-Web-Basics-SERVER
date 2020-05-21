namespace SIS.HTTP.Exceptions
{
    using System;

    public class InternalServerErrorException : Exception
    {
        private const string INTERNAL_SERVER_ERROR_DEFAULT_MESSAGE = "The Server has encountered an error.";

        public InternalServerErrorException()
            : this(INTERNAL_SERVER_ERROR_DEFAULT_MESSAGE)
        {
        }

        public InternalServerErrorException(string message)
            : base(message)
        {
        }
    }
}
