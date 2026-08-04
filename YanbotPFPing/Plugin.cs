using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace YanbotPFPing;

/// <summary>
/// Zero-configuration companion plugin for Yan-bot. Install it and it
/// works out of the box: no config window, no webhook URL to paste in.
/// The only control is a simple on/off toggle via "/yanbot on"/"/yanbot
/// off", for players who'd rather enable it only when they're stepping
/// away. See README.md for the full design.
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
            HelpMessage = "Shows Yanbot PF Ping's status, or use \"/yanbot on\"/\"/yanbot off\" to toggle notifications.",
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
        switch (args.Trim().ToLowerInvariant())
        {
            case "on":
            case "enable":
                Service.Configuration.Enabled = true;
                Service.Configuration.Save();
                Service.ChatGui.Print(
                    "[Yanbot PF Ping] Enabled - you'll be pinged when someone else fills your party to 8/8.");
                return;
            case "off":
            case "disable":
                Service.Configuration.Enabled = false;
                Service.Configuration.Save();
                Service.ChatGui.Print(
                    "[Yanbot PF Ping] Disabled - no notifications until you run \"/yanbot on\" again.");
                return;
        }

        var isAfk = CharacterUtil.IsClientAfk();
        Service.ChatGui.Print(
            $"[Yanbot PF Ping] {(Service.Configuration.Enabled ? "Enabled" : "Disabled")}. "
            + $"AFK right now: {(isAfk ? "yes" : "no")} (informational only). "
            + "Use \"/yanbot on\" or \"/yanbot off\" to toggle notifications.");
    }
}
