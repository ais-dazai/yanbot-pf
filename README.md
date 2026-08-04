# Yanbot PF Ping

Zero-configuration Dalamud plugin. While you're AFK, if your Party Finder
group fills up to 8/8 because someone else joined, it tells the
[Duskbound Discord bot](https://github.com/ais-dazai/yanbot-discord),
which pings you in a Discord channel - if you've linked your character with
`/register`. If you haven't registered, the bot has no Discord account to
ping, so nothing happens.

There is nothing to configure. Install it and it just works.

## How it decides when to notify

- Your cross-world party (the same thing a Party Finder group is) reaches
  8/8 members.
- The member who completed it is someone **else**, not you - if you're the
  one who just joined an already-organized party as the final member,
  that's not "your listing filled", so it stays quiet.
- You are currently AFK (same "Zzz" status the game itself shows).

All three conditions must be true. See `CrossWorldPartyListSystem.cs` and
`PartyListener.cs` for the exact logic.

## ⚠️ Important: this hasn't been compiled or tested in-game yet

This plugin was written by an AI assistant working from a Linux sandbox
with no access to Windows, Visual Studio, or a real Dalamud installation -
so none of this code has actually been built or run. It's grounded in two
real, working, current Dalamud plugins (their source was read directly to
get the API calls right):

- [PushyFinder](https://github.com/snightshade/PushyFinder) - the
  cross-world party polling/diffing approach and the AFK check
  (`OnlineStatus.RowId is 17 or 18`) are adapted from its actual
  production code.
- [FFLogsViewer](https://github.com/Aireil/FFLogsViewer) - the
  `InfoProxyCrossRealm` struct access pattern (`CrossRealmGroups`,
  `GroupMembers`, `.HomeWorld`, `.NameString`) and reading the local
  player's own name/world via `IPlayerState` are adapted from its actual
  production code.

Still, Dalamud's API surface shifts between game patches, and this needs a
real build to catch anything that doesn't compile. **The first thing to do
is build it and fix whatever the compiler complains about** - expect that
to be a real possibility, not a sign something went wrong with the plan.

## Building and testing it yourself (no Discord/guild distribution needed yet)

You need [XIVLauncher](https://goatcorp.github.io/) installed and Dalamud
initialized at least once (just launch the game through XIVLauncher once).

1. Install the .NET SDK matching the `TargetFramework` in
   `YanbotPFPing/YanbotPFPing.csproj` (currently `net10.0`).
2. Copy `YanbotPFPing/Secrets.cs.example` to
   `YanbotPFPing/Secrets.cs` and fill in the real API URL/token (ask
   whoever manages the bot's Railway deployment for these - `Secrets.cs`
   is gitignored, never commit it).
3. Open `YanbotPFPing.sln` in Visual Studio (or run
   `dotnet build` from this folder) and build.
4. In-game, open the Dalamud Plugin Installer (`/xlplugins`), go to the
   "Dev Tools" tab (may need "Settings" -> "Experimental" ->
   "Enable dev plugin locations" first), and add the build output folder
   (`YanbotPFPing/bin/x64/Debug` or `/Release`) as a dev plugin
   location. Enable the plugin from there.
5. Run `/yanbotpfping` in chat to confirm it loaded and check your
   live AFK status reading.
6. To test the actual notification: go AFK, and have someone else join
   your Party Finder group until it's full. Check the configured Discord
   channel for the ping, and check the plugin log (`/xllog`) for any
   `PartyFullNotifier` errors if nothing shows up.

Once this is confirmed working, the next step is distributing it to the
rest of the guild via a custom Dalamud plugin repository (a `repo.json` +
a built release, so people can add the repo URL in-game instead of
building it themselves) - not set up yet, since it depends on this build
actually working first.
