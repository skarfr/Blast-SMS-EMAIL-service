using System;
using System.Diagnostics;

namespace Tools
{
    /// <summary>
    /// This class contains static functions related the loging of messages in the windows Event log
    /// </summary>
    public static class EventLogger
    {
        private static string logSource = "Blast";
        private static string logName = "Application";

        /// <summary>
        /// Used to write/create a new entry in the windows Event log
        /// </summary>
        /// <param name="message">the message to be written in the Event log. It will automatically be pre-fixed with the current date and time</param>
        /// <param name="type">the event type: Error, Warning, Information...</param>
        public static void WriteEntry(string message, EventLogEntryType type)
        {  
            EventLog myLog = new EventLog(logName);
            myLog.Source = logSource;
            myLog.WriteEntry(DateTime.Now.ToString() + ": " + message, type);
        }

        /// <summary>
        /// Used to create the Event log source 1 time only.
        /// </summary>
        public static void CreateSource()
        {
            if (!EventLog.SourceExists(logSource)) // If the source doesn't already exist, we create it 
                EventLog.CreateEventSource(logSource, "");
        }
    }
}
