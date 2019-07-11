# What is SensitivityMatcherXAML?
It's a fairly close 1:1 port from KovaaK's Sensitivity Matcher AutoIt script, which you can find [here](https://github.com/KovaaK/SensitivityMatcher).

It's called SensitivityMatcherXAML because the UI is using WPF and therefore uses XAML.

# Why is this a thing then?
Well, I'm always looking for new small projects, so I can program a few things that I like.

And since (afaik) there are some Anti-Cheat Tools out there that don't seem to like AutoHotkey (and I'm just assuming that might be the case for AutoIt aswell),
I thought I could just port this over to C#.

# Usage and example:
This is a tool that will allow you to easily convert your mouse sensitivity from one game to another, without having to rely on any third party website or person!

See this video from KovaaK himself :)
https://youtu.be/PBHAsvVU55s

# Missing features
- Unless I forgot something, most basic features should be included

# Current Hotkeys
* Numpad 2 = Turn a lot
* Numpad 4 = Turn less
* Numpad 5 = Turn once
* Numpad 6 = Turn more
* Numpad 8 = Clear Bounds (This is still buggy IIRC)

# Disclaimer
This MIGHT be usable with games that do block/ban AutoIT-scripts, but obviously I can neither guarantee it, nor will I attempt to work around those limitations.
On top of that, I'm NOT responsible for any bans or other issues you may run into.
Use at your own risk!

# Used Nuget Packages
- Costura.Fody
- PropertyChanged.Fody

# Credit
- KovaaK
