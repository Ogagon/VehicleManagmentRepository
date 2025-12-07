using System;

namespace MonoTask.Service.Interfaces
{
    public interface ILogger
    {
        void Info(string message);
        void Warn(string message, Exception ex = null);
        void Error(string message, Exception ex = null);
    }
}
