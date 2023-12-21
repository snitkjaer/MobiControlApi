using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NLog;

namespace MobiControlApi
{
    public class LogAbstraction
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public void Log(string message, SeverityLevel severityLevel)
        {
            logger.Log(MapSeverityLevelToNLogLevel(severityLevel), message);
        }


        public void TrackException(Exception ex)
        {
            logger.Error(ex, "ex");;
        }

        public static string DCSeriallNumberToString(uint ser)
        {
            return "device certificate S/N " + ser.ToString() + "(0x" + ser.ToString("X") + ")";
        }

        public static NLog.LogLevel MapSeverityLevelToNLogLevel(SeverityLevel severityLevel)
        {
            switch (severityLevel)
            {
                case SeverityLevel.Verbose:
                    return NLog.LogLevel.Debug; // Or LogLevel.Trace, if you prefer more detailed logging
                case SeverityLevel.Information:
                    return NLog.LogLevel.Info;
                case SeverityLevel.Warning:
                    return NLog.LogLevel.Warn;
                case SeverityLevel.Error:
                    return NLog.LogLevel.Error;
                case SeverityLevel.Critical:
                    return NLog.LogLevel.Fatal;
                default:
                    throw new ArgumentException("Invalid severity level");
            }
        }


        public void TrackEvent(string eventName, TimeSpan elapsed, IDictionary<string, string> properties = null)
        {
            logger.Info(" [Event] - " + eventName + " in " + elapsed.Milliseconds.ToString() + " ms " + string.Join(";", properties));   

        }
    }

    public enum SeverityLevel
    {
        Verbose,
        Information,
        Warning,
        Error,
        Critical
    }



}
