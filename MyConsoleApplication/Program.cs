using DataMapper;
using DomainModel;
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



            //try
            //{
            //    Console.WriteLine("Starting application...");
            //    var serviceProvider = OnStart.OnStart.ConfigureServices();
            //    Console.WriteLine("Application configured successfully!");

            //    // TEST LOGGING
            //    Console.WriteLine("\n=== TESTING LOGGING ===");
            //    var authorService = serviceProvider.GetRequiredService<IAuthorService>();

            //    try
            //    {
            //        // This should fail and log an error
            //        authorService.AddAuthor(null);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Exception caught as expected: {ex.Message}");
            //    }

            //    Console.WriteLine("\nCheck SQL Server: SELECT * FROM ApplicationLogs");
            //    Console.WriteLine("You should see an error log entry!");

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
                Console.WriteLine("Starting Library Management System...");
                var serviceProvider = OnStart.OnStart.ConfigureServices();
                Console.WriteLine("Application configured successfully!\n");

                // Test 1: Validator error handling
                Console.WriteLine("Test 1: Validator Error");
                var authorService = serviceProvider.GetRequiredService<IAuthorService>();
                try
                {
                    authorService.AddAuthor(null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Validator working: {ex.Message}\n");
                }

                // Test 2: Add real author (cu proprietățile CORECTE)
                Console.WriteLine("Test 2: Add Real Author");
                using (var scope = serviceProvider.CreateScope())
                {
                    var realService = scope.ServiceProvider.GetRequiredService<IAuthorService>();

                    var newAuthor = new Author
                    {
                        FirstName = "Ionica",
                        LastName = "Creanga",
                        Age = 45
                    };

                    try
                    {
                        realService.AddAuthor(newAuthor);
                        Console.WriteLine("Author added successfully\n");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"AddAuthor failed: {ex.Message}\n");
                    }
                }

                // Test 3: Check database content
                Console.WriteLine("Test 3: Database Content");
                using (var dbScope = serviceProvider.CreateScope())
                {
                    var context = dbScope.ServiceProvider.GetRequiredService<LibraryContext>();

                    var authorCount = context.Authors.Count();
                    Console.WriteLine($"Authors in database: {authorCount}");

                    if (authorCount > 0)
                    {
                        var firstAuthor = context.Authors.First();
                        Console.WriteLine($"First author: {firstAuthor.FirstName} {firstAuthor.LastName}, Age: {firstAuthor.Age}");
                    }
                }

                Console.WriteLine("\nSQL Server verification:");
                Console.WriteLine("USE LibraryDB;");
                Console.WriteLine("SELECT * FROM ApplicationLogs ORDER BY Id DESC;");
                Console.WriteLine("SELECT * FROM Authors;");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical error: {ex.Message}");
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
