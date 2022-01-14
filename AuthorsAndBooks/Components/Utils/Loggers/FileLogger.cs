using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace AuthorsAndBooks.Components.Utils.Loggers
{
    public class FileLogger : ILogger
    {
        private static object accessLocker = new object();

        private readonly string filePath;

        public FileLogger(IWebHostEnvironment webHostEnvironment)
        {
            filePath = Path.Combine(webHostEnvironment.ContentRootPath, "Resources", "Log.txt");
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                lock (accessLocker)
                {
                    File.AppendAllText(filePath, formatter(state, exception) + string.Concat(Enumerable.Repeat(Environment.NewLine, 2)));
                }
            }
        }
    }
}