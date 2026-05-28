using NetCord;
using NetCord.Rest;

namespace DiscordBotNumber23.Services;

//theoretically would probably be worth to also make different colors - so turn this into some service maybe idk for like alerts

public class MyEmbeds : EmbedProperties
{
    private const string FooterText = "Made by zx111";
    private static readonly Color MyColour = new Color(255, 0, 0);

    public MyEmbeds()
    {
        Footer = new EmbedFooterProperties()
        {
            Text =  FooterText,
            IconUrl = "https://images-ext-1.discordapp.net/external/ZynAwX5Ta4Kvb9WvoQxIpS1aKDGmrD-I8FO3TxNF7lA/https/cdn.discordapp.com/avatars/360803113864658944/dffe50f9f9c309a8f2d4cc2dd1ef1c55.png?format=webp&quality=lossless"
        };
        Color = MyColour;
        Timestamp = DateTime.Now;
    }
}