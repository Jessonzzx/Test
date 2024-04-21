using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading;

public class CustomFileLogger : ILogger
{
    private static readonly object _lock = new object();
    private readonly string _filePath;

    public CustomFileLogger(string filename)
    {
        // Here we're setting the log file to be in the user's application data folder
        string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _filePath = Path.Combine(folder, $"{filename}.log");
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        // Scopes can be used to provide contextual information to the logs, but we're not using them here.
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    { 
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }
 
        if (formatter == null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        string message = formatter(state, exception);
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        if (exception != null)
        {
            message += $"\nException: {exception}";
        }

        // Ensure only one thread can write to the file at a time
        lock (_lock)
        {
            File.AppendAllText(_filePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{Thread.CurrentThread.ManagedThreadId}] {logLevel}: {message}\n");
        }
    }
}