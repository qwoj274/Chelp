using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Debugger
{
    public delegate void Logger(string message, DebugTypes debugType =  DebugTypes.DEBUG);
    public enum DebugTypes
    {
        DEBUG,
        WARNING,
        ERROR,
    }
    public static class Debug
    {
        const string LOG_DIR_PATH = "./Debug";
        const string LOG_FILENAME = "log.txt";
        static readonly string LOGFILE_FULLPATH = Path.Combine(LOG_DIR_PATH, LOG_FILENAME);
        static readonly string LOG_FILENAME_NO_EXTENSION = Path.GetFileNameWithoutExtension(LOG_FILENAME);

        static readonly StreamWriter _debugFile;

        static Debug()
        {
            Directory.CreateDirectory(LOG_DIR_PATH);

            FileInfo fileInfo = new(LOGFILE_FULLPATH);
            bool isFileExistsAndEmpty = fileInfo.Exists && Convert.ToBoolean(fileInfo.Length);

            _debugFile = File.AppendText(LOGFILE_FULLPATH);

            if (!isFileExistsAndEmpty)
            {
                LogNoFormat("====== WELLCOME TO CHELPER ======");
            } else
            {
                LogNoFormat("");
            }
        }
        static readonly Dictionary<DebugTypes, string> stringLogTypes = new()
        {
            { DebugTypes.DEBUG, "DBG" },
            { DebugTypes.WARNING, "WRN" },
            { DebugTypes.ERROR, "ERR" },
        };
        public static string GetCurrentTimeFormat()
        {
            DateTime currentTime = DateTime.Now;
            string formattedTime = currentTime.ToString(@"[dd\/MM\/yyyy HH:mm:ss.ff]");
            return formattedTime;
        }
        public static void Log(string value, DebugTypes debugType)
        {
            string content = $"{GetCurrentTimeFormat()} [{stringLogTypes[debugType]}]: {value}";
            _debugFile.WriteLine(content);
            _debugFile.Flush();
        }
        public static void Log(string value)
        {
            Log(value, DebugTypes.DEBUG);
        }
        public static void LogNoFormat(string value)
        {
            _debugFile.WriteLine(value);
            _debugFile.Flush();
        }
    }
}