using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ServiceLayer;
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
            //try
            //{
            //    Console.WriteLine("Starting application...");

            //    var serviceProvider = OnStart.OnStart.ConfigureServices();

            //    Console.WriteLine("Application configured successfully!");
            //    Console.WriteLine("Database should now be created.");
            //    Console.WriteLine("\nPress any key to exit...");
            //    Console.ReadKey();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"ERROR: {ex.Message}");
            //    Console.WriteLine($"\nDetails: {ex}");
            //    Console.ReadKey();
            //}
            //finally
            //{
            //    Log.CloseAndFlush();
            //}
            try
            {
                Console.WriteLine("Starting application...");
                var serviceProvider = OnStart.OnStart.ConfigureServices();
                Console.WriteLine("Application configured successfully!");

                // TEST LOGGING
                Console.WriteLine("\n=== TESTING LOGGING ===");
                var authorService = serviceProvider.GetRequiredService<IAuthorService>();

                try
                {
                    // This should fail and log an error
                    authorService.AddAuthor(null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception caught as expected: {ex.Message}");
                }

                Console.WriteLine("\nCheck SQL Server: SELECT * FROM ApplicationLogs");
                Console.WriteLine("You should see an error log entry!");

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
