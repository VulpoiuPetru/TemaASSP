using DataMapper;
using DataMapper.RepoServiceCollectionMethod;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceLayer;
using ServiceLayer.ServiceServiceCollectionMethod;




namespace MyConsoleApplication.OnStart
{
    /// <summary>
    /// Provides startup configuration and dependency injection setup for the library management system.
    /// </summary>
    public class OnStart
    {
        /// <summary>
        /// Configures all services and initializes the database.
        /// </summary>
        /// <returns>Configured service provider.</returns>
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Add logging without SQL sink initially
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(dispose: true);
            });

            // Register all services following layered architecture requirements
            services.AddInfrastructureServices();
            services.AddServiceLayerServices();
            services.AddScoped<LibraryContext>();

            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                // Step 1: Create database and all tables using EF6 Code-First
                var context = scope.ServiceProvider.GetRequiredService<LibraryContext>();
                if (!context.Database.Exists())
                {
                    context.Database.CreateIfNotExists();
                    Console.WriteLine("Database and tables created successfully.");
                }
                else
                {
                    Console.WriteLine("Database already exists.");
                }

                // Step 2: Configure Serilog SQL logging after database creation
                ConfigureLogging();
                Console.WriteLine("Serilog SQL logging configured.");
            }

            return serviceProvider;
        }

        /// <summary>
        /// Configures Serilog with console and SQL Server sinks for comprehensive logging.
        /// Logs are written to ApplicationLogs table with custom columns.
        /// </summary>
        private static void ConfigureLogging()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["LibraryDBConnectionString"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("LibraryDBConnectionString is not configured in app.config");
            }

            var sqlSinkOptions = new MSSqlServerSinkOptions
            {
                TableName = "ApplicationLogs",
                AutoCreateSqlTable = true,
                BatchPostingLimit = 50,
                BatchPeriod = TimeSpan.FromSeconds(2)
            };

            var columnOptions = new ColumnOptions
            {
                AdditionalColumns = new Collection<SqlColumn>
                {
                    new SqlColumn { ColumnName = "MachineName", DataType = SqlDbType.NVarChar, DataLength = 50 },
                    new SqlColumn { ColumnName = "SourceContext", DataType = SqlDbType.NVarChar, DataLength = 128 }
                }
            };

            columnOptions.Store.Remove(StandardColumn.Properties);
            columnOptions.Store.Remove(StandardColumn.Id);

            // Configure logger with console (Information+) and SQL (Warning+)
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: sqlSinkOptions,
                    restrictedToMinimumLevel: LogEventLevel.Warning,
                    columnOptions: columnOptions)
                .CreateLogger();
        }
    }
}
