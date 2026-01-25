using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Swift;
using NetCord;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace HorseCommands;

public class HorseModule : ApplicationCommandModule<ApplicationCommandContext>
{
    private static Random r = new();

    // Cream of the Crop
    [SlashCommand("horse", "🐎")]
    public static async Task<InteractionMessageProperties> GetHorse()
    {
        var randomHorseByte = await Horse.Instance.GetRandomHorse();

        return new InteractionMessageProperties()
        {
            Embeds = new []
            {
                new EmbedProperties
                {
                    Color = new Color(0xFFFFFF),
                    Title = "🐎🐎🐎",
                }
            },
            Attachments =
            [
                new AttachmentProperties("horse.jpg",
                    new MemoryStream(randomHorseByte)
                )
            ] 
        };
    }

    [SlashCommand("loadhorse", "Loads the horses in.")]
    public static async Task<string> LoadHorse()
    {
        bool isLoaded = await Horse.Instance.LoadHorseList();
        if (!isLoaded)
        {
            return "Failed To Load The Horses...";
        }
        return "Horse Locked and Loaded";
    } 
        
    [SlashCommand("checkhorse", "checks whether or not the horses are loaded")]
    public static string CheckHorse()
    {
        string message = "";
        if (Horse.Instance._horselist == null || Horse.Instance._horselist.Length == 0)
        {
            message = "No horse loaded :(";
            return message;
        }
        
        message = "Horses Locked and loaded! 🐎:\n";
        foreach (string img in Horse.Instance._horselist)
        {
            message += $"* {img}\n";
        }
        return message;
    }

    [SlashCommand("checkapi", "checks whether or not the bot is connected to the HorseAPI")]
    public static async Task<InteractionMessageProperties> CheckAPI()
    {
        List<string> apiStatuses = await Horse.Instance.IsApiOnline();
        string message = "";

        foreach (string status in apiStatuses)
        {
            message += $"* {status}\n";
        }
        
        return new InteractionMessageProperties()
        {
            Embeds =
            [
                new EmbedProperties
                {
                    Title = "Server HorseAPI status",
                    Description = $"**{message}**"
                }
            ] 
        };
    }
}


/// <summary>
/// The Horse Class that has Horse-related codes and function
/// </summary>
/// <remarks>
/// Very cool because its made by carbon.dev
/// </remarks>
public class Horse
{
    // singleton
    private static readonly Horse _instance = new();

    /// <returns>
    /// The only instance of the class
    /// </returns>
        public static Horse Instance => _instance;
        private readonly HttpClient _http = new();
        private Horse() {} // Private for Singleton purposes

        private static string? activeAddress;
        private readonly ConcurrentDictionary<string, byte[]> _cache = new();
        public string[]? _horselist;
        private string[] apiList = ["http://192.168.1.16:5000", "http://192.168.1.8:5000"];


        public async Task<bool> LoadHorseList()
        {
            if (_horselist != null && _horselist.Length > 0)
                return true;

            try
            {
                for (int i = 0 ; i < apiList.Length ; i++)
                {
                    try
                    {                    
                        _horselist = await _http.GetFromJsonAsync<string[]>($"{apiList[i]}/HorseList");
                        activeAddress = apiList[i];
                        return _horselist != null && _horselist.Length > 0;
                    }
                    catch
                    {
                        continue;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<byte[]> GetRandomHorse()
        {
            byte[] imageBytes;
            // makes sure to load the horses before execution
            if (_horselist == null || _horselist.Length == 0)
                await LoadHorseList();
            
            var name = _horselist![Random.Shared.Next(_horselist.Length)];

            // checks if its already in cache
            if (_cache.TryGetValue(name, out var cached))
                return cached;

            try
            {
                imageBytes = await _http.GetByteArrayAsync($"{activeAddress}/img/{name}");
            }
            catch
            {
                throw new InvalidOperationException("Failed to connect to CarbonAPI");
            }

            _cache[name] = imageBytes;
            return imageBytes;
        }

    /// <returns>text of the api that is currently reachable</returns>
    public async Task<List<string>> IsApiOnline()
    {
        List<string> apiStatuses = new();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        for (int i = 0 ; i < apiList.Length ; i++)
        {
            HttpResponseMessage status; 
            try
            {
                status = await _http.GetAsync
                (
                    $"{apiList[i]}/foo",
                    cts.Token
                );

                var text = await status.Content.ReadAsStringAsync(cts.Token);

                if (text == "bar")
                {
                    apiStatuses.Add($"🟩 {apiList[i]}   : Online");
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                apiStatuses.Add($"🟥 {apiList[i]}   : Down / Unreachable");
            }
        }

        return apiStatuses;
    }
}