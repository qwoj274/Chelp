using System;

namespace Main
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Option option = new Option("test2", "third");
            Console.WriteLine(option.Value);
        }
    }
}