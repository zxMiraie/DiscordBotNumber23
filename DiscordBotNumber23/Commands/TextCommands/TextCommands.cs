using DiscordBotNumber23.Services;
using NetCord.Rest;
using NetCord.Services.Commands;
using System.IO;

namespace DiscordBotNumber23.Commands.TextCommands;

public class TextCommands(YtDownloader ytDownloader) : CommandModule<CommandContext>
{
    private const ulong Owner = 360803113864658944;
    [Command("profile")]
    public async Task Funny()
    {
        var user = Context.User;
        var embed = new MyEmbeds
        {
            Title = $"Profile of {user.Username}"
        };
        if (Context.Channel != null) await Context.Channel.SendMessageAsync(new MessageProperties { Embeds = [embed] });
    }



    [Command("ytdownload")]
    public async Task Youtube(string url)
    {
        if (!url.Contains("youtube.com"))
        {
            await Context.Message.ReplyAsync(new ReplyMessageProperties { Content = "You need to include Youtube.com" });
            return;
        }

        if (Context.User.Id != Owner)
        {
            await Context.Message.ReplyAsync(new ReplyMessageProperties { Content = "Only zx can use this lol" });
            return;
        }

        var message = await Context.Message.ReplyAsync(new ReplyMessageProperties { Content = "Downloading..." });

        var filePath = await ytDownloader.DownloadVideo(url);

        if (filePath == null) 
        {
            await message.ModifyAsync(x => x.Content = "Couldn't download video");
            return;
        }

        await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        if (Context.Channel != null)
            await Context.Channel.SendMessageAsync(new MessageProperties { Content = "Video Downloaded" }
                .WithAttachments([new AttachmentProperties(Path.GetFileName(filePath), fileStream)]));
    }
}