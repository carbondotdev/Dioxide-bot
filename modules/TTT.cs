using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Swift;
using NetCord;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace TicTacToe;

public class TictacToeModule : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("pingt", "test the ttt module")]
    public static InteractionMessageProperties PingTTT () => new()
    {
        Content = "From the TicTacToe Module:",
        Embeds = new[]
        {
            new EmbedProperties
            {
                Title = "Pong! 🏓",
                Description = "Pong from the TicTacToe Module!",
                Color = new Color(0x0dd675) 
            }
        }
    };
}