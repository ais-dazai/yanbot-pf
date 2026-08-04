using Dalamud.Configuration;

namespace YanbotPFPing;

/// <summary>
/// Persisted plugin settings, saved through Dalamud's standard per-plugin
/// config store (no custom file, no settings window). Currently just the
/// on/off toggle exposed via "/yanbot on" and "/yanbot off" - lets a
/// player silence notifications while actively at the keyboard and turn
/// them back on right before stepping away, and the choice survives
/// relogging/game restarts.
/// </summary>
internal class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    /// <summary>
    /// Defaults to true so the plugin still notifies out of the box for
    /// anyone who never touches "/yanbot on"/"/yanbot off" - matches the
    /// zero-configuration install-and-go design.
    /// </summary>
    public bool Enabled { get; set; } = true;

    public void Save() => Service.PluginInterface.SavePluginConfig(this);
}
