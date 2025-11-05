using Debugger;
using System;
using System.Collections.Generic;

namespace IniParser
{
    public class IniSection(string name)
    {
        public string Name { get; private set; } = name;
        private readonly Dictionary<string, object> _data = [];
        public Dictionary<string, object> Data => _data;
        public static IniSection Empty => new("");
        public object? this[string key] => GetValue<object>(key);

        public void AddValue(string key, string value)
        {
            object parsedValue;
            if (bool.TryParse(value, out bool booleanResult))
                parsedValue = booleanResult;
            else if (int.TryParse(value, out int numericResult))
                parsedValue = numericResult;
            else
                parsedValue = value;
            _data.Add(key, parsedValue);
            IniFile.logger?.Invoke($"{GetType().Name}: successfully added [{key}] = {parsedValue.GetType()}:[{parsedValue}] in secton [{Name}]", DebugTypes.DEBUG);
        }
        private bool CheckForKey(string key)
        {
            if (!_data.ContainsKey(key))
            {
                IniFile.logger?.Invoke($"{GetType().Name}: key [{key}] is not found in [{Name}] section!", DebugTypes.ERROR);
                return false;
            }
            return true;
        }
        public Type GetTypeByKey(string key)
        {
            if (!CheckForKey(key)) { return typeof(object); }
            return _data[key].GetType();
        }
        public T? GetValue<T>(string key)
        {
            if (!CheckForKey(key))
                return default;

            try
            {
                return (T)Convert.ChangeType(_data[key], typeof(T));
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}