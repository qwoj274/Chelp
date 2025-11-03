using System;
using System.Collections.Generic;
using System.IO;
using Debugger;

namespace IniParser
{
    public class IniFile
    {
        private readonly string _fullpath = "";
        private List<string> _lines = [];
        private List<IniSection> _sections = new();

        public IniFile(string fullpath)
        {
            if (!File.Exists(fullpath))
            {
                Debug.Log($"{GetType().Name}: file doesn't exist on {fullpath}!", DebugTypes.ERROR);
                throw new FileNotFoundException($"File is not exist on {fullpath}");
            }

            if (!Path.GetExtension(fullpath).ToUpper().Equals(".INI", StringComparison.CurrentCultureIgnoreCase))
            {
                Debug.Log($"{GetType().Name}: given file is not .ini!", DebugTypes.ERROR);
                throw new FileNotFoundException("Given file is not .INI!");
            }

            _fullpath = fullpath;

            ReadLines();
            Parse();
        }

        private void ReadLines()
        {
            string[] lines = File.ReadAllLines(_fullpath);
            foreach (string line in lines)
            {
                if (line.StartsWith("#") || line.StartsWith(";"))
                {
                    continue;
                }
                _lines.Add(line);
            }
        }

        private void Parse()
        {
            bool isThereAtleastOneSection = false;
            int currentSectionIndex = -1;

            foreach (var line in _lines)
            {
                if (line == string.Empty) { continue; }

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    isThereAtleastOneSection = true;
                    currentSectionIndex++;
                    string sectionName = line.Trim('[', ']');
                    _sections.Add(new(sectionName));
                    continue;
                }

                if (isThereAtleastOneSection)
                {
                    IniSection currentSection = _sections[currentSectionIndex];
                    string[] keyAndValue = line.Split('=');
                    if (keyAndValue.Length != 2)
                    {
                        Debug.Log($@"{GetType().Name}: line doesn't match ""key=value"" pattern and will be skipped!", DebugTypes.ERROR);
                        continue;
                    }
                    string key = keyAndValue[0].Trim();
                    string value = keyAndValue[1].Trim();
                    Debug.Log($"{GetType().Name}: extracted: [{key}] = [{value}] from section [{currentSection.Name}]", DebugTypes.DEBUG);
                    currentSection.AddValue(key, value);
                    continue;
                }
                Debug.Log($"{GetType().Name}: there is no sections in file {_fullpath}", DebugTypes.ERROR);
                break;
            }
        }

        #nullable enable
        public T? GetValue<T>(string sectionName, string key)
        {
            IniSection? desiredSection = null;

            foreach (IniSection section in _sections)
            {
                if (section.Name == sectionName)
                {
                    desiredSection = section;
                    break;
                }
            }

            if (desiredSection == null)
            {
                Debug.Log($"{GetType().Name}: section [{sectionName}] not found! value will be default for {typeof(T).Name}", DebugTypes.WARNING);
                return default;
            }
            return desiredSection.GetValue<T>(key);
        }
    }
}