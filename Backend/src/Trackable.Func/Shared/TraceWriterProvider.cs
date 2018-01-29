using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;

namespace Trackable.Func.Shared
{
    public class TraceWriterProvider : ILoggerProvider
    {
        private readonly TraceWriter writer;
        private readonly Func<string, LogLevel, bool> filter;

        public TraceWriterProvider(TraceWriter writer, Func<string, LogLevel, bool> filter)
        {
            this.writer = writer;
            this.filter = filter;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TraceWriterLogger(categoryName, writer, filter);
        }

        public void Dispose()
        {
        }
    }
}
