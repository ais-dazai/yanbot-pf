namespace YanbotPFPing;

internal static class CharacterUtil
{
    /// <summary>
    /// Same AFK check PushyFinder uses in production: the game itself sets
    /// the player's OnlineStatus to one of these two row IDs when idle (the
    /// "Zzz" icon), so there's no need to track idle time ourselves.
    /// 17 = AFK, 18 = Camera Mode/gpose (also effectively idle).
    /// Square Enix has occasionally renumbered online statuses across
    /// patches before - if this stops matching, use "/yanbot" in-game to
    /// check the live AFK reading while testing. Not currently used to
    /// gate notifications (those fire regardless of AFK state), kept only
    /// for the informational status shown by the "/yanbot" command.
    /// </summary>
    public static bool IsClientAfk()
    {
        if (!Service.ClientState.IsLoggedIn || Service.ObjectTable.LocalPlayer == null)
            return false;

        return Service.ObjectTable.LocalPlayer.OnlineStatus.RowId is 17 or 18;
    }
}
