namespace YanbotPFPing;

internal static class PartyListener
{
    public static void On() => CrossWorldPartyListSystem.OnPartyFull += OnPartyFull;

    public static void Off() => CrossWorldPartyListSystem.OnPartyFull -= OnPartyFull;

    private static void OnPartyFull() => PartyFullNotifier.NotifyPartyFull();
}
