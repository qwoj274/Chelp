using Debugger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Compilation
{
    internal class CppFinder
    {
        private readonly Logger logger = Debug.Log;

        private List<string> _cppFiles = [];

        public string CppFiles
        {
            get
            {
                string ret = string.Empty;
                int index = 0;
                foreach (var file in _cppFiles)
                {
                    index++;
                    ret += $"{index}) {Path.GetFileName(file)}\n";
                }
                return ret;
            }
        }

        public CppFinder(string path)
        {
            if (path == null) throw new ArgumentNullException();

            if (!Directory.Exists(path))
            {
                logger("directory doesn't exists!", DebugTypes.ERROR);
                return;
            }

            _cppFiles = Directory.GetFiles(path, "*.cpp").ToList<string>();

            if (_cppFiles.Count == 0)
            {
                logger("there is no .cpp files in directory!", DebugTypes.ERROR);
            }
        }

        public List<string> GetCppFiles()
        {
            return _cppFiles;
        }
    }
}
