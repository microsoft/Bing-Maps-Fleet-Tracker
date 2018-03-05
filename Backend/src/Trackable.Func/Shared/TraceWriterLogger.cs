// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Trackable.Func.Shared
{
    using System;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.Extensions.Logging;

    public class TraceWriterLogger : ILogger
    {
        private string categoryName;
        private TraceWriter writer;
        private Func<string, LogLevel, bool> filter;

        public TraceWriterLogger(string categoryName, TraceWriter writer, Func<string, LogLevel, bool> filter)
        {
            this.writer = writer;
            this.categoryName = categoryName;
            this.filter = filter;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (filter == null || filter(categoryName, logLevel));
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

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $"{ logLevel }: {message}";

            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception.ToString();
            }

            switch (logLevel)
            {
                case LogLevel.Information:
                    writer.Info(message, categoryName);
                    break;
                case LogLevel.Debug:
                    writer.Verbose(message, categoryName);
                    break;
                case LogLevel.Trace:
                    writer.Verbose(message, categoryName);
                    break;
                case LogLevel.Warning:
                    writer.Warning(message, categoryName);
                    break;
                case LogLevel.Critical:
                case LogLevel.Error:
                    writer.Error(message, ex: exception, source: categoryName);
                    break;
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}