using System;
using Microsoft.Extensions.Logging;
using API.PDV.Domain;

namespace API.PDV.Application
{
    public class LoggingService
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
        }

        public void LogError(Exception ex, string message)
        {
            var sanitized = SecurityUtils.SanitizeLogMessage(message);
            _logger.LogError(ex, sanitized);
        }

        public void LogInfo(string message)
        {
            var sanitized = SecurityUtils.SanitizeLogMessage(message);
            _logger.LogInformation(sanitized);
        }
    }
}