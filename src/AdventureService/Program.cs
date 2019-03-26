using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdventureService.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace AdventureService
{
    public class Program
    {
        public static IConfiguration SerilogConfiguration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(SerilogConfiguration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting Adventure Service...");
                var host = CreateWebHost(args);

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    Console.WriteLine("Current Environment: {0}",
                        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
                    DatabaseInitializer.InitializeDatabase(services);
                }

                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Adventure Service terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost CreateWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(SerilogConfiguration)
                .UseSerilog()
                .UseStartup<Startup>()
                .Build();
    }
}
