using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IniParser
{
    public class IniFile
    {
        private readonly string _fullpath = "";
        private readonly List<string> _lines = [];
        private readonly List<IniSection> _sections = [];
        public IniSection this[string section] => GetSection(section);

        public IniFile(string fullpath)
        {
            if (!File.Exists(fullpath))
            {
                throw new FileNotFoundException($"File doesn't exist on {fullpath}");
            }

            if (!Path.GetExtension(fullpath).ToUpper().Equals(".INI", StringComparison.CurrentCultureIgnoreCase))
            {
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
                if (line.StartsWith('#') || line.StartsWith(';'))
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

                if (line.StartsWith('[') && line.EndsWith(']'))
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
                        continue;
                    }
                    string key = keyAndValue[0].Trim();
                    string value = keyAndValue[1].Trim();
                    currentSection.AddValue(key, value);
                    continue;
                }
                break;
            }
        }
        public T? GetValue<T>(string sectionName, string key)
        {
            Type type = typeof(T);
            Type[] allowedTypes = [typeof(int), typeof(bool), typeof(string)];

            if (!allowedTypes.Contains(type))
            {
                return default;
            }

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
                return default;
            }
            return desiredSection.GetValue<T>(key);
        }
        public IniSection GetSection(string sectionName)
        {
            foreach (IniSection section in _sections)
            {
                if (section.Name == sectionName)
                {
                    return section;
                }
            }

            return IniSection.Empty;
        }
    }
}