using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chelp
{
    public class Option
    {
        private const string OptionFilePath = "settings.ini";
        private static IniParser.Ini OptionFile;
        private static List<Option> List = [];
        public string Name { get; private set; }
        public object? Value { get; private set; }

        static Option()
        {
            OptionFile = new IniParser.Ini(OptionFilePath);
        }
        public Option(string section, string name)
        {
            Name = name;
            Value = OptionFile.GetValue<object>(section, name);

            if (Value == null)
            {
                Debug.Log($"cannot find parameter witn key {name}, value will be null.", DebugTypes.WARNING);
            }
            
            List.Add(this);
        }
    }

    class IniParser
    {
        public class IniSection(string name)
        {
            public string Name { get; private set; } = name;
            private Dictionary<string, object> data = [];
            public Dictionary<string, object> Data { get { return data; } }

            public void AddValue(string key, string value)
            {
                object parsedValue;

                if (bool.TryParse(value, out bool booleanResult))
                    parsedValue = booleanResult;
                else if (int.TryParse(value, out int numericResult))
                    parsedValue = numericResult;
                else
                    parsedValue = value;

                data.Add(key, parsedValue);
                Debug.Log($"{GetType().Name}: successfully added [{key}] = {parsedValue.GetType()}:[{parsedValue}] in secton [{Name}]", DebugTypes.DEBUG);
            }

            private bool CheckForKey(string key)
            {
                if (!data.ContainsKey(key))
                {
                    Debug.Log($"{GetType().Name}: key [{key}] is not found in [{Name}] section!", DebugTypes.ERROR);
                    return false;
                }
                return true;
            }

            public Type GetTypeByKey(string key)
            {
                if (!CheckForKey(key)) { return typeof(object); }

                return data[key].GetType();
            }

            public T? GetValue<T> (string key)
            {
                if (!CheckForKey(key))
                    return default; 

                return (T)data[key];
            }
        }


        public class Ini
        {
            private readonly string _fullpath = "";
            private List<string> _lines = [];
            private List<IniSection> _sections = new();

            public Ini(string fullpath)
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

            public T? GetValue<T>(string sectionName, string key)
            {
                IniSection? desiredSection = null;

                foreach (IniSection section in _sections)
                {
                    Console.WriteLine(section);
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
}