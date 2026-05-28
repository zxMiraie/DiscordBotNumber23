using NetCord.Services.ApplicationCommands;

namespace DiscordBotNumber23.SlashCommands;

public class SlashCommands : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("ping", "Pong")]
    public static string Ping() => "Pong!";

    [SlashCommand("CurrentTime", "Current Time")]
    public static string CurrentTime()
    {
        return DateTime.Now.ToLongTimeString();
    }
}