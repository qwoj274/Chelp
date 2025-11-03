using System.Collections.Generic;
using IniParser;
using Debugger;
public class Option
{
    private const string OptionFilePath = "settings.ini";
    private static IniFile OptionFile;
    private static List<Option> List = [];
    public string Name { get; private set; }
    public object? Value { get; private set; }
    static Option()
    {
        OptionFile = new IniFile(OptionFilePath);
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
