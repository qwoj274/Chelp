using ConsoleApp.Compilation;
using System;
using System.Diagnostics;
using System.IO;

namespace ConsoleApp
{
    public class Program
    {
        static Compiler? choosenCompiler;
        public static void Main(string[] _)
        {
            CompilerFinder.SearchForCompiler();
            var compilers = CompilerFinder.compilers;

            int index = 0;

            if (compilers.Count == 0)
            {
                Console.WriteLine("no compilers");
                Console.ReadKey();
                return;
            } 

            foreach (var compiler in compilers)
            {
                index++;
                Console.Write($"{index}) {compiler.Name} : {compiler.Description} : {compiler.Version}");
                Console.WriteLine();
            }

            short choice = Convert.ToInt16(Console.ReadLine());
            choosenCompiler = compilers[choice-1];

            CppFinder cppFinder = new("C:\\Users\\nikita\\Documents\\EX");
            Console.WriteLine(cppFinder.CppFiles);

            Directory.CreateDirectory("Build");

            choosenCompiler.Compile("Build\\a.exe", null, cppFinder.GetCppFiles());

            Process output = new Process()
            {
                StartInfo = {
                    FileName = "Build\\a.exe",
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                }
            };

            output.Start();
            StreamReader reader = output.StandardOutput;
            output.WaitForExit();
            
            string stringReader = reader.ReadToEnd();
            Console.WriteLine(stringReader);

            Console.ReadKey();
        }
    }
}