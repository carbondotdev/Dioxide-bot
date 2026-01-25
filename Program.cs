using NetCord;
using NetCord.Gateway;
using NetCord.Logging;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;
using Sprache;
using TestCommands;

// Bot Token
DotNetEnv.Env.Load();
string TOKEN = Environment.GetEnvironmentVariable("DISCORD_TOKEN")
    ?? throw new Exception("DISCORD_TOKEN is not set");

// Gateway
GatewayClient client = new(new BotToken(TOKEN), new GatewayClientConfiguration
{
    Logger = new ConsoleLogger(),
});


// Interaction Service
ApplicationCommandService<ApplicationCommandContext> acs = new();
acs.AddModules(typeof(Program).Assembly);

ComponentInteractionService<ComponentInteractionContext> cis = new();
cis.AddModules(typeof(Program).Assembly);

// Handler
client.InteractionCreate += async interaction =>
{
    // Slash Command Handler
    if (interaction is ApplicationCommandInteraction slashCmd)
    {
        var result = await acs.ExecuteAsync(new ApplicationCommandContext(slashCmd, client));

        if (result is IFailResult failResult)
        {
            await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
        }
    }

    if (interaction is ComponentInteraction cmpnt)
    {
        var result = await cis.ExecuteAsync(new ComponentInteractionContext(cmpnt, client));
        
        if (result is IFailResult failResult)
        {
            await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
        }
    }
};

await acs.RegisterCommandsAsync(client.Rest, client.Id);

await client.StartAsync();
await Task.Delay(-1);
