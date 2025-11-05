using Debugger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApp.Compilation
{
    public class Compiler
    {
        private Logger logger = Debugger.Debug.Log;
        private const string COMPILER_TEST_DIRECTORY = "Assets\\CompilerTest";
        private const string COMPILER_TEST_CPP = "test.cpp";

        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsValid { get; private set; } = false;
        public readonly string Fullpath = string.Empty;
        public readonly string Version = string.Empty;

        public Compiler(string name, string description, string fullpath)
        {
            Name = name;
            Description = description;
            if (!Path.Exists(fullpath))
            {
                IsValid = false;
                return;
            }
            Fullpath = fullpath;
            Description = description;
            Version = GetVersion();
        }
        internal bool SelfTest()
        {
            string input = Path.Combine(COMPILER_TEST_DIRECTORY, COMPILER_TEST_CPP);
            string output = Path.ChangeExtension(input, ".exe");

            var compilationExitcode = Compile(output, [], input);

            if (compilationExitcode != 0)
            {
                logger($"compilation with {Name} failed!", DebugTypes.ERROR);
                return false;
            }

            Process compilerTestProcess = new()
            {
                StartInfo = {
                    FileName = output,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                }
            };

            using (compilerTestProcess)
            {
                compilerTestProcess.Start();
                var outputStream = compilerTestProcess.StandardOutput;
                compilerTestProcess.WaitForExit();

                var outputString = outputStream.ReadLine();
                File.Delete(output);

                if (outputString != "TEST STRING")
                {
                    logger($"{Name} failed test!");
                    IsValid = false;
                } else
                {
                    logger($"{Name} passed test!");
                    IsValid = true;
                }
            }

            return IsValid;
        }
        public string GetVersion()
        {
            Process getCompilerVersionProcess = new()
            {
                StartInfo =
                {
                    FileName = Fullpath,
                    Arguments = "--version",
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                }
            };

            getCompilerVersionProcess.Start();
            StreamReader reader = getCompilerVersionProcess.StandardOutput;
            getCompilerVersionProcess.WaitForExit();
            string version = reader.ReadLine()?.Split(' ').Last() ?? string.Empty;
            return version;
        }
        public int Compile(string outputFile, string[]? arguments, params List<string> inputFiles)
        {
            IEnumerable<string> SelectValidFiles(List<string> filesList)
            {
                foreach (string file in filesList) {
                    bool isValidCpp = File.Exists(file);
                    if (isValidCpp)
                    {
                        yield return file;
                    }
                }
            };

            List<string> filesToCompile = [.. SelectValidFiles(inputFiles)];
            List<string> commandParts = [];
            commandParts.AddRange(filesToCompile);

            if (arguments != null)
            {
                commandParts.AddRange(arguments);
            }

            commandParts.AddRange("-o", $"{outputFile}");

            Process compilationProcess = new()
            {
                StartInfo = {
                    FileName = $"\"{Path.GetFileName(Fullpath)}\"",
                    UseShellExecute = false,
                    Arguments = String.Join(" ", commandParts),
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                }
            };

            using (compilationProcess)
            {
                compilationProcess.Start();
                compilationProcess.WaitForExit();
                var exitCode = compilationProcess.ExitCode;
                var errors = compilationProcess.StandardError.ReadToEnd().ReplaceLineEndings("\r\n").Split("\r\n");
                if (exitCode == 0)
                {
                    logger($"{Name}: compilation was successful!");
                } else
                {
                    logger($"{Name}: some errors occured during compilation:", DebugTypes.ERROR);
                    foreach (var error in errors)
                    {
                        logger(error, DebugTypes.ERROR);
                    }
                }
                return exitCode;
            }
        }
    }
}