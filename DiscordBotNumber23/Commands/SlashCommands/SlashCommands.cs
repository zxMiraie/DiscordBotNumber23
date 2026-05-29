using DiscordBotNumber23.Services;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using System.IO;

namespace DiscordBotNumber23.Commands.SlashCommands;

public class SlashCommands(PetPetService petPet, ShipService ship) : ApplicationCommandModule<ApplicationCommandContext>
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

    [SlashCommand("petpet", "Pet a user")]
    public async Task PetPet(
        [SlashCommandParameter(Name = "user", Description = "The user to pet")] User user)
    {
        // Generating the GIF takes longer than Discord's 3s window, so defer first.
        await RespondAsync(InteractionCallback.DeferredMessage());

        var avatarUrl = AvatarUrl(user, 256);
        await using var gif = await petPet.CreateAsync(avatarUrl);

        await FollowupAsync(new InteractionMessageProperties
        {
            Content = $"<@{Context.User.Id}> pets <@{user.Id}>",
            Attachments = [new AttachmentProperties("petpet.gif", gif)]
        });
    }

    [SlashCommand("avatar", "Show a user's avatar")]
    public async Task Avatar(
        [SlashCommandParameter(Name = "user", Description = "The user (defaults to you)")] User? user = null)
    {
        var target = user ?? Context.User;
        var embed = new MyEmbeds
        {
            Title = $"{target.Username}'s avatar",
            Image = new EmbedImageProperties(AvatarUrl(target, 1024))
        };
        await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties { Embeds = [embed] }));
    }

    [SlashCommand("ship", "Ship two users together")]
    public async Task Ship(
        [SlashCommandParameter(Name = "user1", Description = "First user")] User user1,
        [SlashCommandParameter(Name = "user2", Description = "Second user")] User user2)
    {
        await RespondAsync(InteractionCallback.DeferredMessage());

        await using var image = await ship.CreateAsync(AvatarUrl(user1, 256), AvatarUrl(user2, 256));

        // Deterministic so the same pair always gets the same score, regardless of order.
        var (lo, hi) = user1.Id <= user2.Id ? (user1.Id, user2.Id) : (user2.Id, user1.Id);
        var percent = (int)(unchecked(lo * 1000003UL + hi) % 101UL);

        var name1 = user1.GlobalName ?? user1.Username;
        var name2 = user2.GlobalName ?? user2.Username;
        var shipName = ShipName(name1, name2);

        var filled = percent / 10;
        var bar = new string('█', filled) + new string('░', 10 - filled);

        var content =
            $"**{name1}** 💞 **{name2}**\n" +
            $"Ship name: **{shipName}**\n" +
            $"❤️ **{percent}%**\n" +
            $"{bar}";

        await FollowupAsync(new InteractionMessageProperties
        {
            Content = content,
            Attachments = [new AttachmentProperties("ship.png", image)]
        });
    }

    [SlashCommand("decide", "Yes, no, or maybe?")]
    public async Task Decide(
        [SlashCommandParameter(Name = "question", Description = "What should I decide?")] string? question = null)
    {
        string[] answers = ["Yes ✅", "No ❌", "Maybe 🤔"];
        var answer = answers[Random.Shared.Next(answers.Length)];

        var content = question is null ? $"**{answer}**" : $"> {question}\n**{answer}**";
        await RespondAsync(InteractionCallback.Message(content));
    }

    private static string AvatarUrl(User user, int size) =>
        (user.GetAvatarUrl(ImageFormat.Png) ?? ImageUrl.DefaultUserAvatar(user.Id)).ToString(size);

    private static string ShipName(string a, string b)
    {
        var first = a.Length > 1 ? a[..(a.Length / 2)] : a;
        var second = b.Length > 1 ? b[(b.Length / 2)..] : b;
        return first + second;
    }
}
