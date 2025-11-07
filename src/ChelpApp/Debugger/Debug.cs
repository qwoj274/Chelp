using System.IO;
using System.Windows;

namespace ChelpApp.Debugger
{
    public class DebugPublisher()
    {
        public delegate void DebugEventHandler(string message);

        public event DebugEventHandler? DebugEvent;

        public void Debug(string message)
        {
            OnDebugEvent(message);
        }
        protected virtual void OnDebugEvent(string message)
        {
            if (Application.Current?.Dispatcher.CheckAccess() == false)
            {
                Application.Current.Dispatcher.Invoke(() => DebugEvent?.Invoke(message));
            }
            else
            {
                DebugEvent?.Invoke(message);
            }
        }
    }


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
        public static string LogFileFullpath { get; private set; } = LOGFILE_FULLPATH;

        static StreamWriter _debugFile;

        public static DebugPublisher debugPublisher = new();

        static Debug()
        {
            Directory.CreateDirectory(LOG_DIR_PATH);

            FileInfo fileInfo = new(LOGFILE_FULLPATH);
            bool isFileExistsAndEmpty = fileInfo.Exists && Convert.ToBoolean(fileInfo.Length);

            _debugFile = File.AppendText(LOGFILE_FULLPATH);

            if (!isFileExistsAndEmpty)
            {
                LogNoFormat("====== WELCOME TO CHELP ======");
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
            if (_debugFile == null)
            {
                return;
            }
            _debugFile.WriteLine(content);
            _debugFile.Flush();
            debugPublisher.Debug(content);
        }
        public static void Log(string value)
        {
            Log(value, DebugTypes.DEBUG);
        }
        public static void LogNoFormat(string value)
        {
            if (_debugFile == null)
            {
                return;
            }
            _debugFile.WriteLine(value);
            _debugFile.Flush();
        }
        public static void ResetLog()
        {
            _debugFile.Close();
            File.Delete(LOGFILE_FULLPATH);
            _debugFile = File.AppendText(LOGFILE_FULLPATH);
            LogNoFormat("====== WELCOME TO CHELP ======");
        }
    }
}