using log4net;
using MonoTask.Service.Interfaces;
using System;

namespace MonoTask.MVC.Logging
{
    public class Logger : ILogger
    {
        private readonly ILog _log;
        public Logger(Type type)
        {
            _log = LogManager.GetLogger(type);
        }
        public void Info(string message) => _log.Info(message);
        public void Warn(string message, Exception ex)
        {
            if (ex == null) _log.Warn(message);
            else _log.Warn(message, ex);

        }
        public void Error(string message, Exception ex = null)
        {
            if (ex == null) _log.Error(message);
            else _log.Error(message, ex);
        }
    }
}