using System.Collections.Concurrent;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Swift;
using NetCord;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;
using Sprache;

namespace TicTacToe;

public class TictacToeModule : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("ttt", "Start a Tic Tac Toe game session you can play with other people!")]
    public InteractionMessageProperties TicTacToe()
    {
        ulong playerId = Context.User.Id;
        var gameId = Guid.NewGuid().ToString()[..8];

        // Message Properties
        var startEmbed = new EmbedProperties
        {
            Title = "❌ Tic Tac Toe ⭕",
            Color = new(0xff2e2e),  
            Description =   $"**<@{playerId}> is looking for an opponent**\n" +
                            $"❌: <@{playerId}>\n⭕: looking for an opponent...",
            Footer = new() { Text = $"Game ID: {gameId}" }
        };

        // Buttons
        var challengeButton = new ButtonProperties(
            $"challenge:{playerId}:{gameId}",
            "Challenge Them!",
            EmojiProperties.Standard("⚔️"),
            ButtonStyle.Danger);
        
        var aiButton = new ButtonProperties(
            $"challenge-ai:{playerId}:{gameId}",
            "Challenge a bot",
            EmojiProperties.Standard("🤖"),
            ButtonStyle.Primary
        );

        // return
        return new()
        {
            Embeds = [startEmbed],
            Components = [new ActionRowProperties { Components = [challengeButton, aiButton]}]
        };
    }
}

// Buttons

public class TicTacToeButtons : ComponentInteractionModule<ComponentInteractionContext>
{
    [ComponentInteraction("challenge")]
    public InteractionMessageProperties ChallengeHandler(ulong playerId1, string gameId)
    {
        ulong playerId2 = Context.User.Id;

        var gameEmbed =  new EmbedProperties
        {
            Title = "❌ Tic Tac Toe ⭕",
            Color = new Color(0x00eb2f),
            Description =   $"**<@{playerId2}> has challenged <@{playerId1}>**\n" +
                            $"***into a Showdown!\n\n***" +
                            $"❌: <@{playerId1}>\n" +
                            $"⭕: <@{playerId2}>",
            
            Footer = new()
            {
                Text = $"Game Id: {gameId}"
            }
        };

        var errorEmbed = new EmbedProperties
        {
            Title = "Error",
            Description = "Can't challenge yourself, idiot",
            Color = new Color(0xfa0000)
        };

        if (playerId2 == playerId1)
        {            
            return new(){Embeds = [errorEmbed], Flags = MessageFlags.Ephemeral};
        }

        return new InteractionMessageProperties
        {
            Embeds = [gameEmbed]
        };
    }

    [ComponentInteraction("challenge-ai")]
    public InteractionMessageProperties ChallengeAIHandler(ulong playerId1, string gameId)
    {
        var aiEmbed = new EmbedProperties
        {
            Title = "_Note from the Dev_",
            Color = new Color(0xffffff),
            Image = "https://cdn.discordapp.com/attachments/603132749339164683/1464497924422766775/image.png?ex=697700fa&is=6975af7a&hm=95b5a4cf3fb8ca796c86a13fa8064d88928b85ecc7bfbf7d28ef6f1a1bb858c3",
            Description = "\t_-carbon.dev_",
            Timestamp = DateTime.UtcNow
        };

        var errorEmbed = new EmbedProperties
        {
            Title = "Error",
            Color = new Color(0xfa0000),
            Description = "You're not the initializer, idiot!"
        };

        if (Context.User.Id != playerId1)
        {
            return new()
            {
              Embeds = [errorEmbed],
              Flags = MessageFlags.Ephemeral
            };
        }
        
        return new()
        {
            Embeds = [aiEmbed]
        };
    }

    private void DeleteRespondAfterDelay(int ms)
    {
        var interaction = Context.Interaction;

        _ = Task.Run(async () =>
        {
            await Task.Delay(ms);

            try
            {
                await interaction.DeleteResponseAsync();
            } 
            catch {}
        });
    }
}


/// <summary>
/// Tic Tac Toe game logic 
/// </summary>
public class TTT
{
    private char[,] board;

    public TTT()
    {
        board = new char[3,3];
        Reset();
    }

    private void Reset()
    {
        for (int col = 0 ; col < 3 ; col++)
        {
            for (int row = 0; row < 3; row++)
            {
                board[col,row] = ' ';
            }
        }
    }

    public bool MakeMove(int col, int row, bool player)
    {
        char[] symbols =  {'X', 'O'};

        char currentSymbol;
        if (player == true)
        {
            currentSymbol = symbols[0];
        }
        else
        {
            currentSymbol = symbols[1];
        }

        // checks if the position is valid
        if (row > 2 || col > 2)
            return false;

        // checks if the position is empty
        if (board[col, row] != ' ')
            return false;

        board[col, row] = currentSymbol;
        return true;
    }

    public void Display()
    {
        Console.WriteLine($" {board[0,0]} | {board[0,1]} | {board[0,2]} ");
        Console.WriteLine("-----------");
        Console.WriteLine($" {board[1,0]} | {board[1,1]} | {board[1,2]} ");
        Console.WriteLine("-----------");
        Console.WriteLine($" {board[2,0]} | {board[2,1]} | {board[2,2]} ");   
    }

    public bool CheckWinner()
    {
        // Check Horizontal
        for (int col = 0; col < 3 ; col++)
        {
            if (board[col, 0] != ' ' &&
                board[col, 1] == board[col,0] &&
                board[col, 2] == board[col, 1])
            {
                return true;
            }
    }

        // Check Vertically
        for (int row = 0; row < 3; row++)
        {
            if (board[0, row] != ' '&&
                board[1,row] == board[0,row]&&
                board[2, row] == board[1, row])
            {
                return true;
            }
        }

        // Checks Diagonally from
        // top-left to bottom right
        if (board[0,0] != ' '&&
            board[1,1] == board[0,0]&&
            board[2,2] == board[1,1])
        {
            return true;
        }
        
        // Checks Diagonally from
        // top-right to bottom-left
        if (board[0,2] != ' '&&
            board[1,1] == board[0,2]&&
            board[2,0] == board[1,1])
        {
            return true;
        }

        return false;
    }    
}