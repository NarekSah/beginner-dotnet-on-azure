using System.Diagnostics;

namespace MunsonPickles.Web.Services;

public class LoggingService
{
    private readonly ILogger<LoggingService> _logger;

    public LoggingService(ILogger<LoggingService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Logs the execution time of the provided action
    /// </summary>
    /// <param name="actionName">Name of the action being performed</param>
    /// <param name="action">The action to execute</param>
    /// <returns>The result of the action</returns>
    public T LogExecutionTime<T>(string actionName, Func<T> action)
    {
        _logger.LogInformation("Starting {ActionName}", actionName);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = action();
            stopwatch.Stop();
            
            _logger.LogInformation("{ActionName} completed successfully in {ElapsedMilliseconds}ms", 
                actionName, stopwatch.ElapsedMilliseconds);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "{ActionName} failed after {ElapsedMilliseconds}ms", 
                actionName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    /// <summary>
    /// Logs the execution time of the provided asynchronous action
    /// </summary>
    /// <param name="actionName">Name of the action being performed</param>
    /// <param name="action">The asynchronous action to execute</param>
    /// <returns>The result of the action</returns>
    public async Task<T> LogExecutionTimeAsync<T>(string actionName, Func<Task<T>> action)
    {
        _logger.LogInformation("Starting {ActionName}", actionName);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await action();
            stopwatch.Stop();
            
            _logger.LogInformation("{ActionName} completed successfully in {ElapsedMilliseconds}ms", 
                actionName, stopwatch.ElapsedMilliseconds);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "{ActionName} failed after {ElapsedMilliseconds}ms", 
                actionName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    /// <summary>
    /// Logs an application event
    /// </summary>
    /// <param name="eventType">Type of event</param>
    /// <param name="message">Event message</param>
    /// <param name="properties">Additional properties to include in the log</param>
    public void LogApplicationEvent(string eventType, string message, Dictionary<string, object>? properties = null)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["EventType"] = eventType
               }))
        {
            if (properties != null)
            {
                using (_logger.BeginScope(properties))
                {
                    _logger.LogInformation("{EventMessage}", message);
                }
            }
            else
            {
                _logger.LogInformation("{EventMessage}", message);
            }
        }
    }
}