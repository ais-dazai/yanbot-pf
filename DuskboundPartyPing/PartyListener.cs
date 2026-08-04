namespace DuskboundPartyPing;

internal static class PartyListener
{
    public static void On() => CrossWorldPartyListSystem.OnPartyFull += OnPartyFull;

    public static void Off() => CrossWorldPartyListSystem.OnPartyFull -= OnPartyFull;

    private static void OnPartyFull()
    {
        if (!CharacterUtil.IsClientAfk())
            return;

        PartyFullNotifier.NotifyPartyFull();
    }
}
