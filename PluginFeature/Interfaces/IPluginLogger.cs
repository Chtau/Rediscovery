using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Feature.Plugin.Interfaces
{
    public interface IPluginLogger
    {
        void LogTrace(string message, params object[] args);
        void LogTrace(Exception exception, string message, params object[] args);
        void LogCritical(string message, params object[] args);
        void LogCritical(Exception exception, string message, params object[] args);
        void LogDebug(string message, params object[] args);
        void LogDebug(Exception exception, string message, params object[] args);
        void LogError(string message, params object[] args);
        void LogError(Exception exception, string message, params object[] args);
        void LogInformation(string message, params object[] args);
        void LogInformation(Exception exception, string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogWarning(Exception exception, string message, params object[] args);
    }
}
