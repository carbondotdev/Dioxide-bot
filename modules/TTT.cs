using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
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
    /// <summary>
    /// Just for testing
    /// </summary>
    /// <returns>an Embed</returns>
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

    [SlashCommand("ttt", "Start a Tic Tac Toe game session you can play with other people!")]
    public InteractionMessageProperties TicTacToe()
    {
        ulong playerId = Context.User.Id;
        var gameId = Guid.NewGuid().ToString()[..8];

        // Message Properties
        var startEmbed = new EmbedProperties
        {
            Title = $"❌ Tic Tac Toe ⭕",
            Color = new(0xff2e2e),  
            Description =   $"**<@{playerId}> is looking for an opponent**\n" +
                            $"**to challenge him in a __Tic Tac Toe Showdown!__**\n\n"+
                            $"❌: <@{playerId}>\n⭕: looking for an opponent...",
            Footer = new() { Text = $"Game ID: {gameId}" }
        };

        var challengeButton = new ButtonProperties(
            $"challenge-btn-{playerId}",
            "Challenge Them!",
            EmojiProperties.Standard("⚔️"),
            ButtonStyle.Danger);

        // return
        return new()
        {
            Embeds = [startEmbed],
            Components = [new ActionRowProperties { Components = [challengeButton]}]
        };
    }
}

/// <summary>
/// The middle man between the discord interface and the game logic
/// </summary>
public class GameManager
{
    
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