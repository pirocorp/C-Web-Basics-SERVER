namespace SIS.HTTP.Logging
{
    using System;
    using System.IO;

    public class FileLogger : ILogger
    {
        /// <summary>
        /// Don't use it in multithreading environment
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message)
        {
            File.AppendAllLines("log.txt", new []{ $"[{DateTime.UtcNow}] {message}" });
        }
    }
}
