using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YanbotPFPing;

/// <summary>
/// Tells the Duskbound Discord bot's party-finder API (see this project's
/// sibling repo, ffxiv-duskbound-bot/party_api.py) that the local player's
/// party just filled up. The bot does the rest: it looks up who registered
/// this character via /register and pings them - this plugin never talks
/// to Discord directly, and doesn't know or care who owns the account.
/// </summary>
internal static class PartyFullNotifier
{
    private static readonly HttpClient Client = new() { Timeout = TimeSpan.FromSeconds(10) };

    public static void NotifyPartyFull()
    {
        var characterName = Service.PlayerState.CharacterName;
        var homeWorld = Service.PlayerState.HomeWorld.ValueNullable?.Name.ToString();

        if (string.IsNullOrEmpty(characterName) || string.IsNullOrEmpty(homeWorld))
        {
            Service.PluginLog.Warning(
                "PartyFullNotifier: couldn't read local player name/world, skipping notification");
            return;
        }

        // Fire-and-forget: this runs on the framework thread and must not
        // block the game waiting on a network call.
        Task.Run(() => SendAsync(characterName, homeWorld));
    }

    private static async Task SendAsync(string characterName, string server)
    {
        if (string.IsNullOrEmpty(Secrets.ApiToken) || Secrets.ApiToken == "REPLACE_ME")
        {
            Service.PluginLog.Warning(
                "PartyFullNotifier: Secrets.ApiToken isn't set - see Secrets.cs.example. Not sending.");
            return;
        }

        try
        {
            var body = JsonSerializer.Serialize(new { character_name = characterName, server });
            using var request = new HttpRequestMessage(HttpMethod.Post, Secrets.ApiUrl)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Secrets.ApiToken);

            using var response = await Client.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync();
            Service.PluginLog.Debug($"PartyFullNotifier: {(int)response.StatusCode} {responseText}");
        }
        catch (Exception e)
        {
            Service.PluginLog.Error($"PartyFullNotifier: failed to notify the bot: {e.Message}");
        }
    }
}
