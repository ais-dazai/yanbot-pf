using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace YanbotPFPing;

/// <summary>
/// Dalamud service locator - populated automatically by
/// <c>pluginInterface.Create&lt;Service&gt;()</c> in Plugin's constructor.
/// Same pattern used by PushyFinder and FFLogsViewer (the two plugins this
/// one's code was grounded in).
/// </summary>
internal class Service
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IPlayerState PlayerState { get; private set; } = null!;
    [PluginService] internal static IObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] internal static IPluginLog PluginLog { get; private set; } = null!;

    /// <summary>
    /// Not IoC-injected like the properties above - Dalamud doesn't manage
    /// plugin config objects that way. Set manually in Plugin's constructor
    /// right after <c>pluginInterface.Create&lt;Service&gt;()</c>, then read
    /// from here by anything that needs the on/off toggle (e.g.
    /// PartyListener).
    /// </summary>
    internal static Configuration Configuration { get; set; } = null!;
}
