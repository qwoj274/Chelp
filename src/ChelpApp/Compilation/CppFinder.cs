using ChelpApp.Debugger;
using System.IO;

namespace ChelpApp.Compilation
{
    internal class CppFinder
    {
        private readonly Logger logger = Debug.Log;

        private List<FileInfo> _cppFiles = [];

        public string CppFilesAsNumeredList
        {
            get
            {
                string ret = string.Empty;
                int index = 0;
                foreach (var file in _cppFiles)
                {
                    index++;
                    ret += $"{index}) {Path.GetFileName(file.Name)}\n";
                }
                return ret;
            }
        }

        public CppFinder(string path)
        {
            _cppFiles = new List<FileInfo>();
            if (path == null) throw new ArgumentNullException();

            if (!Directory.Exists(path))
            {
                logger("directory doesn't exists!", DebugTypes.ERROR);
                return;
            }

            string[] cppFilesStringArray = Directory.GetFiles(path.Trim(' '), "*.cpp");

            foreach (string cppFile in cppFilesStringArray)
            {
                _cppFiles.Add(new FileInfo(cppFile));
                logger($"found {cppFile}!", DebugTypes.DEBUG);
            }

            if (_cppFiles.Count == 0)
            {
                logger("there is no .cpp files in directory!", DebugTypes.ERROR);
            }
        }

        public List<FileInfo> GetCppFiles()
        {
            return _cppFiles;
        }
    }
}
