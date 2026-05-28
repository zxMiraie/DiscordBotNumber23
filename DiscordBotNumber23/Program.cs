using DiscordBotNumber23.Commands.SlashCommands;
using Microsoft.Extensions.Hosting;
using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.Commands;
using DiscordBotNumber23.Commands.TextCommands;
using DiscordBotNumber23.Services;
using NetCord.Gateway;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton(builder.Configuration);
builder.Services.AddSingleton<YtDownloader>();

foreach (var file in Directory.GetFiles("temp"))
{
    if (File.GetCreationTime(file) < DateTime.UtcNow.AddHours(-2))
        File.Delete(file);
}

builder.Services
    .AddDiscordGateway(options => 
        options.Intents = GatewayIntents.AllNonPrivileged | GatewayIntents.MessageContent)
    .AddApplicationCommands()
    .AddCommands();

var host = builder.Build();

host.AddModules(typeof(Program).Assembly);

await host.RunAsync();