using System.Reflection;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace TestCommands;

public sealed class ExampleModule : ApplicationCommandModule<ApplicationCommandContext>
{
    private static Random r = new();

    [SlashCommand("ping", "Pong!")]
    public static InteractionMessageProperties Pong() => new()
    {
        Content = "Pong! 🏓",
        Embeds = new[]
        {
            new EmbedProperties
            {
                Color = new Color(0xFFFFFF),
                Title = "# Pong! 🏓",
                Description = "## Connected to API\n## Latency : 0.6769420 ms",
                Image = "https://i.pinimg.com/736x/d2/b4/e9/d2b4e9750d41f1478bb78ee7aa10d90e.jpg"
            }
        }
    };

    [SlashCommand("say", "Make the bot say anything!")]
    public static InteractionMessageProperties Say(string msg) => new()
    {
        Content = msg
    };
    [SlashCommand("message", "sends the message from carbon")]
    public static string Message()
    {
        string msg = "";
        switch (r.Next(0,4))
        {
            case 0:
                msg = "I love you! 0";
                break;
            case 1:
                msg = "Fuck you! 1";
                break;
            case 2:
                msg = "Youre Cool! 2";
                break;
            case 3:
                msg = "Balls 3";
                break;
        }
        return msg;
    }
}
