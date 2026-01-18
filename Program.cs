using NetCord;
using NetCord.Gateway;
using NetCord.Logging;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
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


// Command Service
ApplicationCommandService<ApplicationCommandContext> acs = new();
acs.AddModules(typeof(Program).Assembly);

// Handler
client.InteractionCreate += async interaction =>
{
    // checks if the interaction is a application command
    if (interaction is not ApplicationCommandInteraction aci)
        return;

    var result = await acs.ExecuteAsync(new ApplicationCommandContext(aci, client));

    if (result is not IFailResult failResult)
        return;

    try
    {
        await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
    } catch {}
};

await acs.RegisterCommandsAsync(client.Rest, client.Id);

await client.StartAsync();
await Task.Delay(-1);
