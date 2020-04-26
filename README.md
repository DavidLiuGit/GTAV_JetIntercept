# Jets When Wanted

Scramble jets (of your choice, including add-on jets) to attack the player when the player is wanted and in a helicopter/plane.

---
## Installation
Copy `CustomWantedAirTraffic.dll` and `CustomWantedAirTraffic.ini` to your `/scripts` directory

### Requirements
ScriptHookVDotNet 3
.NET Runtime 4.8

---
## Usage
Jets are scrambled automatically
The number of jets at each wanted level can be changed in .INI
Models of jets scrambled can be changed in .INI

### Cheats (shortcuts)
In cheats, type:
  - `goto lsia`, `goto zancudo`, or `goto sandyshores` to teleport to an airport
  - `<x>stars` to set wanted level. For example, `3stars` will set your wanted level to 3

---
## Changelog
### 0.2.3:
- added INI setting for spawn distance; default value 500 meters
### 0.2.2 (unreleased):
- added cheats to get you back into a dogfight quickly after death. In cheats, type:
  - `goto lsia`, `goto zancudo`, or `goto sandyshores` to teleport to an airport
  - `<x>stars` to set wanted level. For example, `3stars` will set your wanted level to 3
### 0.2.1:
- suppressed "jets needed" debug output
### 0.2:
- renamed file to CustomWantedAirTraffic. Please remove CustomAirTraffic if you have already installed 0.1