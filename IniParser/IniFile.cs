using Debugger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IniParser
{
    public class IniFile
    {
        public static Logger? logger { internal get; set; }

        private readonly string _fullpath = "";
        private readonly List<string> _lines = [];
        private readonly List<IniSection> _sections = [];
        public IniSection this[string section] => GetSection(section);

        public IniFile(string fullpath)
        {
            if (!File.Exists(fullpath))
            {
                logger?.Invoke($"{GetType().Name}: file doesn't exist on {fullpath}!", DebugTypes.ERROR);
                throw new FileNotFoundException($"File doesn't exist on {fullpath}");
            }

            if (!Path.GetExtension(fullpath).ToUpper().Equals(".INI", StringComparison.CurrentCultureIgnoreCase))
            {
                logger?.Invoke($"{GetType().Name}: given file is not .ini!", DebugTypes.ERROR);
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
                        logger?.Invoke($@"{GetType().Name}: line doesn't match ""key=value"" pattern and will be skipped!", DebugTypes.ERROR);
                        continue;
                    }
                    string key = keyAndValue[0].Trim();
                    string value = keyAndValue[1].Trim();
                    logger?.Invoke($"{GetType().Name}: extracted: [{key}] = [{value}] from section [{currentSection.Name}]", DebugTypes.DEBUG);
                    currentSection.AddValue(key, value);
                    continue;
                }
                logger?.Invoke($"{GetType().Name}: there is no sections in file {_fullpath}", DebugTypes.ERROR);
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
                logger?.Invoke($"{GetType().Name}: section [{sectionName}] not found! value will be default for {typeof(T).Name}", DebugTypes.WARNING);
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