using TestMODBUS.Models;
using TestMODBUS.Models.ModbusCommands;

namespace TestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter HEX Value:");
                string value = Console.ReadLine();
                short Value = Convert.ToInt16("0x" + value, 16);


                double ConvertedValue = Value * (10.0 / 32768.0);
                double Rounded = Math.Round(ConvertedValue, 3);
                //double Ranged = Floored / 1000;
                Console.WriteLine(Rounded);
                Console.WriteLine("\n");
            }
        }
    }
}