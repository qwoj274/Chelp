using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Debugger;

namespace ConsoleApp.Compilation
{
    internal static class CompilerFinder
    {
        private static Logger logger = Debug.Log;
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
        public static void SearchForCompiler()
        {
            string? pathEnvVariable = Environment.GetEnvironmentVariable("Path");

            if (pathEnvVariable == null)
            {
                throw new Exception("Failed to get PATH variable!");
            }

            string[] pathsInPathVariable = pathEnvVariable.Split(';').Distinct().ToArray();

            string[] compilersNames = compilersDictionary.Keys.ToArray();

            foreach (string path in pathsInPathVariable)
            {
                logger($"checking {path} . . .");
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
        }
    }
}
