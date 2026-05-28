using Microsoft.Extensions.Hosting;
using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using DiscordBotNumber23.SlashCommands;
using NetCord.Hosting.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddDiscordGateway()
    .AddApplicationCommands();

var host = builder.Build();

host.AddModules(typeof(SlashCommands).Assembly);

await host.RunAsync();