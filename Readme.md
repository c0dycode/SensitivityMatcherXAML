# What is SensitivityMatcherXAML?
It's a fairly close 1:1 port from Kovaak's Sensitivity Matcher AutoIt script, which you can find [here](https://github.com/KovaaK/SensitivityMatcher).

It's called SensitivityMatcherXAML because the UI is using WPF and therefore XAML.

# Why is this a thing then?
Well, I'm always looking for new small projects, so I can program a few things that I like.

And since (afaik) there are some Anti-Cheat Tools out there that don't seem to like AutoHotkey (and I'm just assuming that might be the case for AutoIt aswell),
I thought I could just port this over to C#.

# Additional Features
- AutoUpdates

# Missing features
- Rebinding Hotkeys (UI for this will be added in the future)
- "Physical Stats" from Kovaak's script

# Used Nuget Packages
- Costura.Fody
- PropertyChanged.Fody
- Squirrel.Windows
- NuGet.CommandLine (for developers only)

# Credit
- Kovaak 