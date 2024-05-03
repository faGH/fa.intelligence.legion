﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FrostAura.Intelligence.Legion.Telegram.Managers;
using FrostAura.Intelligence.Legion.Shared.Extensions;

var tokenSource = new CancellationTokenSource();
var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.development.json", optional: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// Bind configuration.
services
    .AddConfiguration(configuration);

// Register dependencies.
services
    .AddServices(configuration);

// Initialize the host.
var serviceProvider = services.BuildServiceProvider();
var host = serviceProvider.GetRequiredService<TelegramManager>();

try
{
    using (host)
    {
        await host.RunAsync(tokenSource.Token);

        while (true)
        {
            Console.ReadLine();
        }
    }

}
catch (Exception ex)
{
    Console.WriteLine($"An error has occured. Shutting down the host. Error: {ex.Message}");
    tokenSource.Cancel();
    throw;
}