using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ChelpApp.Debugger;

namespace ChelpApp.Compilation
{
    public class CompilersCacheLoader
    {
        private Logger logger = Debugger.Debug.Log;
        public bool IsCacheFileExists { get; private set; }
        private string cacheFilePath;
        public CompilersCacheLoader(string cacheFilePath)
        {
            this.cacheFilePath = cacheFilePath;
            IsCacheFileExists = Path.Exists(cacheFilePath);
        }
        public void SaveInCache(List<Compiler> compilers)
        {
            StreamWriter wtr;

            if (!IsCacheFileExists)
            {
                logger($"{cacheFilePath} not found! creating file...");
                if (Path.GetDirectoryName(cacheFilePath) != null)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(cacheFilePath) ?? string.Empty);
                }
            }

            using (wtr = new(cacheFilePath)) {
                IsCacheFileExists = true;
                wtr.Write(JsonConvert.SerializeObject(compilers));
                logger($"cached data in {cacheFilePath}!");
            }
        }
        public List<Compiler>? LoadFromCache()
        {
            if (!IsCacheFileExists)
            {
                logger($"cache not found in {cacheFilePath}!", DebugTypes.WARNING);
                return null;
            }
            List<Compiler>? data;
            using (var rdr = new StreamReader(cacheFilePath))
            {
                string fileContent = rdr.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<Compiler>>(fileContent);
                return data ?? null;
            }
        }
        public bool ResetCache()
        {
            if (!IsCacheFileExists)
            {
                logger("data is not cached yet or already reset"!, DebugTypes.WARNING);
                return false;
            }

            File.Delete(cacheFilePath);
            IsCacheFileExists = false;
            logger("cache reset successfully!");
            return true;
        }
    }
}
