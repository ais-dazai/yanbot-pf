using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace DuskboundPartyPing;

/// <summary>
/// Zero-configuration companion plugin for the Duskbound Discord bot.
/// Install it and it just works: no config window, no webhook URL to
/// paste in, nothing to type. See README.md for the full design.
/// </summary>
public sealed class Plugin : IDalamudPlugin
{
    public string Name => "Duskbound Party Ping";
    private const string CommandName = "/duskboundpartyping";

    private ICommandManager CommandManager { get; init; }

    public Plugin(IDalamudPluginInterface pluginInterface, ICommandManager commandManager)
    {
        pluginInterface.Create<Service>();

        CommandManager = commandManager;
        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Shows Duskbound Party Ping's status - there are no settings to configure.",
        });

        CrossWorldPartyListSystem.Start();
        PartyListener.On();
    }

    public void Dispose()
    {
        PartyListener.Off();
        CrossWorldPartyListSystem.Stop();
        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        var isAfk = CharacterUtil.IsClientAfk();
        Service.ChatGui.Print(
            "[Duskbound Party Ping] Running, no configuration needed. "
            + $"AFK right now: {(isAfk ? "yes" : "no")} "
            + "(notifications only fire while AFK, when someone else fills your party to 8/8).");
    }
}
