using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Info;

namespace YanbotPFPing;

/// <summary>
/// Polls the game's cross-world party info once a frame (the same
/// FFXIVClientStructs API both PushyFinder and FFLogsViewer read party
/// members from - a Party Finder group is a cross-world party) and diffs it
/// against the previous frame to notice when someone joins or leaves.
/// </summary>
internal static class CrossWorldPartyListSystem
{
    public delegate void PartyFullDelegate();

    private static readonly List<CrossWorldMember> Members = new();
    private static List<CrossWorldMember> oldMembers = new();

    /// <summary>
    /// Fires when the party reaches 8/8 because someone ELSE joined - not
    /// when the local player is themselves the one whose join completed an
    /// already-organized party (see the "completedBySelf" check below).
    /// </summary>
    public static event PartyFullDelegate? OnPartyFull;

    public static void Start() => Service.Framework.Update += Update;

    public static void Stop() => Service.Framework.Update -= Update;

    private static bool ListContainsMember(List<CrossWorldMember> list, CrossWorldMember member)
        => list.Any(m => m.Name == member.Name);

    private static unsafe void Update(IFramework framework)
    {
        if (!Service.ClientState.IsLoggedIn)
        {
            oldMembers.Clear();
            return;
        }

        var crossRealmProxy = InfoProxyCrossRealm.Instance();
        if (crossRealmProxy == null || !crossRealmProxy->IsInCrossRealmParty)
        {
            oldMembers.Clear();
            return;
        }

        var localGroupIndex = crossRealmProxy->LocalPlayerGroupIndex;
        if (localGroupIndex < 0 || localGroupIndex >= crossRealmProxy->CrossRealmGroups.Length)
        {
            oldMembers.Clear();
            return;
        }

        // Only the local player's own group - a Party Finder group is a
        // single 8-person cross-realm group. Alliance content forms
        // several of these at once, but we only care about "my" group.
        var group = crossRealmProxy->CrossRealmGroups[localGroupIndex];

        Members.Clear();
        for (var i = 0; i < group.GroupMemberCount; i++)
        {
            var member = group.GroupMembers[i];
            if (member.HomeWorld == -1 || member.NameString == string.Empty)
                continue;

            Members.Add(new CrossWorldMember { Name = member.NameString });
        }

        if (Members.Count != oldMembers.Count)
        {
            var newMembers = Members.Where(m => !ListContainsMember(oldMembers, m)).ToList();

            if (group.GroupMemberCount == 8 && newMembers.Count > 0)
            {
                var localPlayerName = Service.PlayerState.CharacterName;
                var completedBySelf = newMembers.Any(m => m.Name == localPlayerName);

                // If the local player is the one who just joined and that
                // join is what brought the party to 8, this is "you joined
                // someone else's already-full group", not "your listing
                // filled" - stay quiet.
                if (!completedBySelf)
                    OnPartyFull?.Invoke();
            }
        }

        oldMembers = Members.ToList();
    }

    private struct CrossWorldMember
    {
        public string Name;
    }
}
