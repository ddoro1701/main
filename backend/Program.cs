using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(
                    (context, config) =>
                    {
                        var builtConfig = config.Build();
                        var account =
                            Environment.GetEnvironmentVariable("COSMOS_ACCOUNT")
                            ?? throw new ArgumentNullException("COSMOS_ACCOUNT");
                        var key =
                            Environment.GetEnvironmentVariable("COSMOS_KEY")
                            ?? throw new ArgumentNullException("COSMOS_KEY");
                        var databaseName =
                            Environment.GetEnvironmentVariable("COSMOS_DATABASE_NAME")
                            ?? throw new ArgumentNullException("COSMOS_DATABASE_NAME");

                        config.AddInMemoryCollection(
                            new[]
                            {
                                new KeyValuePair<string, string>("CosmosDb:Account", account),
                                new KeyValuePair<string, string>("CosmosDb:Key", key),
                                new KeyValuePair<string, string>(
                                    "CosmosDb:DatabaseName",
                                    databaseName
                                ),
                            }
                        );

                        Console.WriteLine($"Connecting to CosmosDB: {account} !!!!!!!!");
                        Console.WriteLine($"Using Database: {databaseName} !!!!!!!!!!!");
                    }
                )
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://*:80");
                });
    }
}
