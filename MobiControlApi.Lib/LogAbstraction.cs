using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;

namespace MobiControlApi
{
    public class LogAbstraction
    {
        // Alow setting but not directly using as we want this to be an abstraction that works witout it
        public TelemetryClient tc { get; set; }
        public bool LogToConsole = true;

        public void Log(string message, SeverityLevel severityLevel)
        {
            if (LogToConsole)
            {
                if (severityLevel != SeverityLevel.Verbose)
                    Console.WriteLine(DateTime.Now + " [" + severityLevel.ToString() + "] - " + message);
            }

            if (tc != null)
                tc.TrackTrace(message, SeverityLevel.Verbose);
        }

        public TelemetryClient GetTC()
        {
            return tc;
        }

        public void TrackException(Exception ex)
        {
            if (tc != null)
                tc.TrackException(ex);
        }

        public void LogMetric(string name, double value)
        {
            if (tc != null)
                tc.GetMetric(name).TrackValue(value);
        }

        public void TrackEvent(string evenName)
        {
            if (tc != null)
                tc.TrackEvent(evenName);
        }

        public void TrackEvent(string eventName, TimeSpan elapsed, IDictionary<string, string> properties = null)
        {
            if (tc != null)
            {
                var telemetry = new EventTelemetry(eventName);
                telemetry.Metrics.Add("Elapsed", elapsed.TotalMilliseconds);

                if (properties != null)
                {
                    foreach (var property in properties)
                    {
                        telemetry.Properties.Add(property.Key, property.Value);
                    }
                }

                tc.TrackEvent(telemetry);
            }
            if (LogToConsole)
            {
                Console.WriteLine(DateTime.Now + " [Event] - " + eventName + " in " + elapsed.Milliseconds.ToString() + " ms " + string.Join(";", properties));
            }

        }

        public static string DCSeriallNumberToString(uint ser)
        {
            return "device certificate S/N " + ser.ToString() + "(0x" + ser.ToString("X") + ")";
        }
    }



}
