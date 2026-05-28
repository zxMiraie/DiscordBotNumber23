using DiscordBotNumber23.Services;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using System.IO;

namespace DiscordBotNumber23.Commands.SlashCommands;

public class SlashCommands() : ApplicationCommandModule<ApplicationCommandContext>
{

    [SlashCommand("ping", "Pong")]
    public async Task Ping()
    {
        await RespondAsync(InteractionCallback.Message("Pong!"));
    }

    [SlashCommand("time", "Current Time")]
    public async Task Time()
    {
        var now = DateTime.Now;
        await RespondAsync(InteractionCallback.Message("Current Time: " + now.ToString("HH:mm:ss")));
    }
}