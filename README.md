# Yanbot PF Ping

Zero-configuration Dalamud plugin. If your Party Finder group fills up to
8/8 because someone else joined, it tells
[Yan-bot](https://github.com/ais-dazai/yanbot-discord), which pings you in
a Discord channel - if you've linked your character with `/register`. If
you haven't registered, the bot has no Discord account to ping, so nothing
happens.

There is nothing to configure. Install it and it just works, notifying by
default. The only control is a simple on/off switch - see below.

## How it decides when to notify

- Your cross-world party (the same thing a Party Finder group is) reaches
  8/8 members.
- The member who completed it is someone **else**, not you - if you're the
  one who just joined an already-organized party as the final member,
  that's not "your listing filled", so it stays quiet.
- The plugin is currently enabled (see below) - notifications fire
  regardless of AFK state, but you can turn them off entirely.

All conditions must be true. See `CrossWorldPartyListSystem.cs` and
`PartyListener.cs` for the exact logic.

## Turning it on/off

```
/yanbot          - toggles notifications on/off and reports the new status
```

Every run flips the switch - run it once to see it go from enabled to
disabled, run it again to flip back. No arguments needed. Useful if
you'd rather only notify while you're stepping away: run `/yanbot` to
turn it off while actively playing, run it again right before you go AFK
to turn it back on. The setting is saved through Dalamud's normal plugin
config storage, so it persists across relogs and game restarts - it
defaults to **on**
for anyone who never touches the command.

## Credits

Built with reference to two real, working Dalamud plugins (their source
was read directly to get the API calls right):

- [PushyFinder](https://github.com/snightshade/PushyFinder) - the
  cross-world party polling/diffing approach is adapted from its actual
  production code.
- [FFLogsViewer](https://github.com/Aireil/FFLogsViewer) - the
  `InfoProxyCrossRealm` struct access pattern (`CrossRealmGroups`,
  `GroupMembers`, `.HomeWorld`, `.NameString`) and reading the local
  player's own name/world via `IPlayerState` are adapted from its actual
  production code.

Confirmed building and working in-game as of the first real test build.

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
5. Run `/yanbot` in chat to confirm it loaded - it toggles notifications
   and reports the resulting status, so you'll see "Enabled" or
   "Disabled" printed back.
6. To test the actual notification: make sure it's enabled (see step 5),
   then have someone else join your Party Finder group until it's full
   (8/8). Check the configured Discord channel for the ping, and check
   the plugin log (`/xllog`) for any `PartyFullNotifier` errors if nothing
   shows up.

## Distributing to other players (no building required on their end)

This repo doubles as a custom Dalamud plugin repository via
`pluginmaster.json` at the repo root, following the format documented at
[dalamud.dev/plugin-publishing/custom-repositories](https://dalamud.dev/plugin-publishing/custom-repositories/).
Other players add the repo once and install/update from Dalamud's normal
Plugin Installer from then on - no Visual Studio needed on their side.

### Publishing a build (do this once, and again for every future update)

1. In Visual Studio, switch the build configuration dropdown (top toolbar,
   next to the green arrow) from **Debug** to **Release**, then rebuild
   (`Ctrl+Shift+B`). The `DalamudPackager` step wired into
   `YanbotPFPing.csproj` automatically packages the build into
   `YanbotPFPing/bin/x64/Release/YanbotPFPing/latest.zip`.
2. On GitHub, go to `https://github.com/ais-dazai/yanbot-pf/releases/new`.
3. Pick a tag (e.g. `v1.0.0` the first time, `v1.0.1` for the next update,
   etc.), give the release any title, and upload `latest.zip` from step 1
   as the release asset - **keep the filename exactly `latest.zip`**, the
   repo URLs below depend on it.
4. Publish the release. `pluginmaster.json`'s download links always point
   at `.../releases/latest/download/latest.zip`, which GitHub
   automatically resolves to whichever release is newest - so this step
   is the only one needed per update, no need to edit `pluginmaster.json`
   unless the plugin's own `Version` in the `.csproj` changed (bump
   `AssemblyVersion` to match in `pluginmaster.json` when it does, so
   Dalamud shows players an update is available).

### Adding the repo in-game (each player does this once)

1. Open the Dalamud Plugin Installer (`/xlplugins`), click the gear icon
   (Settings), go to the **Experimental** tab.
2. Under **Custom Plugin Repositories**, paste this URL and click the `+`:
   ```
   https://raw.githubusercontent.com/ais-dazai/yanbot-pf/main/pluginmaster.json
   ```
3. Save and close settings. Back in the Plugin Installer's normal list,
   search for **"Yanbot PF Ping"** and click Install - same as any other
   plugin from then on, including automatic update notifications.
