using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace YanbotPFPing;

/// <summary>
/// Zero-configuration companion plugin for Yan-bot. Install it and it
/// works out of the box: no config window, no webhook URL to paste in.
/// The only control is "/yanbot" itself, which toggles notifications
/// on/off and reports the resulting status in chat - no arguments needed.
/// See README.md for the full design.
/// </summary>
public sealed class Plugin : IDalamudPlugin
{
    public string Name => "Yanbot PF Ping";
    private const string CommandName = "/yanbot";

    private ICommandManager CommandManager { get; init; }

    public Plugin(IDalamudPluginInterface pluginInterface, ICommandManager commandManager)
    {
        pluginInterface.Create<Service>();
        Service.Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        CommandManager = commandManager;
        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Toggles notifications on/off and shows the resulting status.",
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
        Service.Configuration.Enabled = !Service.Configuration.Enabled;
        Service.Configuration.Save();

        var status = Service.Configuration.Enabled ? "Enabled" : "Disabled";
        Service.ChatGui.Print($"[Yanbot PF Ping] {status}. Run \"/yanbot\" again to toggle.");
    }
}
