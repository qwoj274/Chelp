using System.IO;
using ChelpApp.Debugger;
using Newtonsoft.Json;

namespace ChelpApp.Compilation
{
    internal static class CompilerFinder
    {
        private static Logger logger = Debug.Log;

        const string CACHE_DIRECTORY = "Cache";
        const string CACHE_FILE_NAME = "Compilers.json";
        static readonly string CACHE_FILE_PATH = Path.Combine(CACHE_DIRECTORY, CACHE_FILE_NAME);
        public static string FullPathToCacheFile { get; private set; } = Path.GetFullPath(CACHE_DIRECTORY);

        static readonly string availableCompilersJsonFilePath = Path.Combine("Assets", "AvailableCompilers.json");
        static readonly Dictionary<string, string> compilersDictionary;
        public static List<Compiler> compilers = [];

        static CompilerFinder()
        {
            if (!File.Exists(availableCompilersJsonFilePath))
            {
                logger("file with available compilers doesn't exists!", DebugTypes.ERROR);
                throw new FileNotFoundException($"File doesn't exists on {availableCompilersJsonFilePath}!");
            }

            string fileContent = File.ReadAllText(availableCompilersJsonFilePath);
            var tempDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContent);

            if (tempDict == null)
            {
                logger($"something went wrong while reading data from {availableCompilersJsonFilePath}", DebugTypes.ERROR);
                throw new JsonReaderException();
            }
            compilersDictionary = tempDict;
        }
        public static List<Compiler> SearchForCompiler()
        {
            var cachedData = LoadFromCache();

            if (cachedData != null)
            {
                logger($"found cached data in {CACHE_FILE_PATH}!");
                compilers = [.. cachedData.Distinct()];
                return compilers;
            }

            compilers = new List<Compiler>();

            string? pathEnvVariable = Environment.GetEnvironmentVariable("Path");

            if (pathEnvVariable == null)
            {
                throw new Exception("Failed to get PATH variable!");
            }

            string[] pathsInPathVariable = pathEnvVariable.Split(';').Distinct().ToArray();

            string[] compilersNames = compilersDictionary.Keys.ToArray();

            foreach (string path in pathsInPathVariable)
            {
                logger($"checking directory: {path}");
                if (!Path.Exists(path))
                {
                    logger($"{path} doesn't exists! skipping.", DebugTypes.WARNING);
                    continue;
                }

                foreach (string compilerName in compilersNames)
                {
                    string compilerPath = Path.Combine(path, compilerName);
                    if (!File.Exists(compilerPath))
                    {
                        continue;
                    }
                    logger($"found {compilerName} in: {path}");
                    string compilerDescription = compilersDictionary[compilerName];

                    Compiler compiler = new(compilerName, compilerDescription, compilerPath);
                    bool _ = compiler.SelfTest();

                    compilers.Add(compiler);
                }
            }
            if (compilers.Count == 0)
            {
                logger("no compilers found!", DebugTypes.ERROR);
                return [];
            }
            SaveInCache();
            return compilers;
        }
        public static void SaveInCache()
        {
            Directory.CreateDirectory(CACHE_DIRECTORY);
            var writer = File.CreateText(CACHE_FILE_PATH);

            var content = JsonConvert.SerializeObject(compilers);
            writer.Write(content);
            writer.Flush();
            writer.Close();
            logger($"caching data to {CACHE_FILE_PATH}");
        }
        public static List<Compiler>? LoadFromCache()
        {
            if (!File.Exists(CACHE_FILE_PATH))
            {
                return null;
            }

            string fileContent = File.ReadAllText(CACHE_FILE_PATH);
            var cachedCompilers = new List<Compiler>();
            cachedCompilers = JsonConvert.DeserializeObject<List<Compiler>>(fileContent);

            if (cachedCompilers?.Count == 0)
            {
                return null;
            }
            return cachedCompilers;
        }
        public static void ResetCache()
        {
            if (!File.Exists(CACHE_FILE_PATH)) {
                logger("data is not cached yet or alreay reset!", DebugTypes.WARNING);
                return;
            }

            Directory.Delete(CACHE_DIRECTORY, true);
            logger("cache has reset successfully!", DebugTypes.DEBUG);
        }
    }
}
