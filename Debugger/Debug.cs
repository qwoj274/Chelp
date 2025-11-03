using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Debugger
{
    public enum DebugTypes
    {
        DEBUG,
        WARNING,
        ERROR,
    }

    public static class Debug
    {
        const string LOGPATH = "./debug";
        const string LOGFILENAME = "log.txt";
        const string LOGFILENPATH = $"{LOGPATH}/{LOGFILENAME}"; 
        static readonly string LOGFILENAMENOEXTENSION = LOGFILENAME.Split(".").Last();
        static FileStream _debugFile;
        static Debug()
        {
            Directory.CreateDirectory(LOGPATH);

            FileInfo fileInfo = new(LOGFILENPATH);
            bool isFileExistsAndEmpty = fileInfo.Exists && Convert.ToBoolean(fileInfo.Length);
            FileMode fileMode = isFileExistsAndEmpty ? FileMode.Append : FileMode.OpenOrCreate;

            _debugFile = new(LOGFILENPATH, fileMode, FileAccess.Write);
            if (isFileExistsAndEmpty) {
                RawLog("\n");
            }
        }

        static readonly Dictionary<DebugTypes, string> stringLogTypes = new()
        {
            { DebugTypes.DEBUG, "DBG" },
            { DebugTypes.WARNING, "WRN" },
            {DebugTypes.ERROR, "ERR" },
        };
        
        public static string GetCurrentTimeFormat()
        {
            DateTime currentTime = DateTime.Now;
            string formattedTime = currentTime.ToString(@"[dd\/MM\/yyyy HH:mm:ss.ff]");
            return formattedTime;
        }

        public static void Log(string value, DebugTypes debugType)
        {
            byte[] content = Encoding.UTF8.GetBytes($"{GetCurrentTimeFormat()} [{stringLogTypes[debugType]}]: {value}\n");
            _debugFile.Write(content, 0, content.Length);
            _debugFile.Flush();
        }
        
        public static void RawLog(string value)
        {
            _debugFile.Write(Encoding.UTF8.GetBytes(value, 0, value.Length));
            _debugFile.Flush();
        }
    }
}