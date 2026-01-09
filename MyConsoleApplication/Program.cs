using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConsoleApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting application...");

                var serviceProvider = OnStart.OnStart.ConfigureServices();

                Console.WriteLine("Application configured successfully!");
                Console.WriteLine("Database should now be created.");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($"\nDetails: {ex}");
                Console.ReadKey();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
