using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace HSPacketAnalyzer.Helpers
{
    internal static class Logging
    {
        private static Logger _singleton;

        public static Logger SetDefault(string logName = nameof(HSPacketAnalyzer))
        {
            if (_singleton == null)
            {
#if DEBUG
                _singleton = Debugging(logName).CreateLogger();
#else
                _singleton = Default(logName).CreateLogger();
#endif
            }

            Log.Logger = _singleton;
            return _singleton;
        }

        public static LoggerConfiguration Default(string logName)
        {
            var conf = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext();

            string logPath = $"{logName}.txt";
            return WithAsyncRollingFile(logPath, LogEventLevel.Information, composeTo: WithConsole(LogEventLevel.Warning, conf));
        }

        public static LoggerConfiguration Debugging(string logName)
        {
            var conf = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext();

            string logPath = $"{logName}.txt";
            return WithAsyncRollingFile(logPath, LogEventLevel.Verbose, composeTo: WithConsole(LogEventLevel.Verbose, conf));
        }

        public static LoggerConfiguration WithConsole(LogEventLevel level, LoggerConfiguration composeTo = null)
        {
            composeTo = composeTo ?? new LoggerConfiguration();

            return composeTo
                .WriteTo.Console(restrictedToMinimumLevel: level);
        }

        public static LoggerConfiguration WithRollingFile(string logPath, LogEventLevel level, bool shareFile = true, LoggerConfiguration composeTo = null)
        {
            composeTo = composeTo ?? new LoggerConfiguration();

            return composeTo
                .WriteTo.File(logPath, restrictedToMinimumLevel: level, shared: shareFile, rollingInterval: RollingInterval.Hour, retainedFileCountLimit: 10);
        }

        public static LoggerConfiguration WithAsyncRollingFile(string logPath, LogEventLevel level, bool shareFile = true, LoggerConfiguration composeTo = null)
        {
            composeTo = composeTo ?? new LoggerConfiguration();

            return composeTo
                .WriteTo.Async(logger =>
                    logger.File(logPath, restrictedToMinimumLevel: level, shared: shareFile, rollingInterval: RollingInterval.Hour, retainedFileCountLimit: 10));
        }
    }
}
