using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace TTG.NavalWar.NWData
{
    public class Logger
    {
        public static string LogDir { get; set; }

        private const string FILE_NAME = "TTG_NAVALWAR_LOG";

        protected enum LogLevel
        {
            LevelDebug,
            LevelWarning,
            LevelError
        }

        protected virtual void Log( LogLevel logLevel, string message )
        {
            if (string.IsNullOrEmpty(LogDir))
            {
                LogDir = Assembly.GetExecutingAssembly().Location;
                LogDir = Path.GetDirectoryName(LogDir);
            }
            if (!LogDir.EndsWith(@"\"))
            {
                LogDir += @"\";
            }
            string filePath = LogDir + FILE_NAME + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                StreamWriter sw = File.AppendText(filePath);
                sw.WriteLine(LogLevelName(logLevel) + " - "
                    + DateTime.Now.ToLongTimeString() + "." + DateTime.Now.Millisecond.ToString()
                    + " ** " + message);
                sw.Close();
            }
            catch (Exception)
            {
                //ignore
            }
        }

        [Conditional( "LOG_DEBUG" )]
        public void LogDebug(string message)
        {
            Log(LogLevel.LevelDebug, message);
        }

        [Conditional( "LOG_WARNING" )]
        public void LogWarning(string message)
        {
            Log(LogLevel.LevelWarning, message);
        }

        [Conditional( "LOG_ERROR" )]
        public void LogError(string message)
        {
            Log(LogLevel.LevelError, message);
        }

        private string LogLevelName(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.LevelDebug:
                    return "Debug";
                //break;
                case LogLevel.LevelWarning:
                    return "Warning";
                //break;
                case LogLevel.LevelError:
                    return "Error";
                //break;
                default:
                    return "Unknown";
                //break;
            }
        }
    }
}
