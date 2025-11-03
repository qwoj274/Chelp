using System;
using System.Collections.Generic;
using Debugger;

namespace IniParser
{
    internal class IniSection(string name)
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
        #nullable enable
        public T? GetValue<T>(string key)
        {
            if (!CheckForKey(key))
                return default;
            try
            {
                return (T)Convert.ChangeType(data[key], typeof(T));
            }
            catch (InvalidCastException)
            {
                return default;
            }
        }
    }
}