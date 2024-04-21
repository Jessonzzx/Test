using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace LoggerLibrary
{
    public class CustomFileLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, CustomFileLogger> _loggers = new ConcurrentDictionary<string, CustomFileLogger>();
        private readonly string _filename;

        public CustomFileLoggerProvider(string filename)
        {
            _filename = filename;
        }

        public ILogger CreateLogger(string categoryName)
        {
            // Get or create the logger for the category name
            return _loggers.GetOrAdd(categoryName, name => new CustomFileLogger(_filename));
        }

        public void Dispose()
        {
            // Disposing the provider should clear the loggers
            _loggers.Clear();
        }
    }
}
