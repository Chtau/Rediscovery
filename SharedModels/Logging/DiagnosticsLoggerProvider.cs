using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Logging
{
    public sealed class DiagnosticsLoggerProvider : ILogger
    {
        #region Singleton
        private static readonly DiagnosticsLoggerProvider instance = new DiagnosticsLoggerProvider();
        static DiagnosticsLoggerProvider()
        {
        }
        private DiagnosticsLoggerProvider()
        {
        }
        public static DiagnosticsLoggerProvider Instance
        {
            get
            {
                return instance;
            }
        }

        public void LogCritical(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void LogDebug(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void LogError(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void LogInformation(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void LogTrace(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void LogWarning(string message, params object[] args)
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
