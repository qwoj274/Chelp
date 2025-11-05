using Debugger;
using IniParser;
using ConsoleApp.Compilation;
using System;

namespace ConsoleApp
{
    public class Program
    {
        public static void Main(string[] _)
        {
            IniFile.logger = Debug.Log;

            CompilerFinder.SearchForCompiler();
            var compilers = CompilerFinder.compilers;

            foreach (var compiler in compilers)
            {
                Console.Write($"{compiler.Name} : {compiler.Description} : {compiler.Fullpath}");
                Console.WriteLine();
            }
        }
    }
}