using System;

namespace Chelp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Option macan = new("test", "macan");

            Console.WriteLine(macan.Value);
        }
    }
}