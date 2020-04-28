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
Jets are scrambled automatically.  
The number of jets at each wanted level can be changed in .INI.  
Models of jets scrambled can be changed in .INI

### Settings
#### Aircraft Only
If `aircraftOnly` setting is `true`, jets will only be scrambled if you are in a helicopter or plane.

#### Below Radar
If `belowRadar` setting is `true`, no jets will be scrambled if you are less than 100 meters above the ground. Any jets already spawned and chasing you will continue to chase you, even if you drop below 100 meters.  
**This setting applies even if `aircraftOnly` is `false`**. If you want jets to be scrambled while you're on foot or in a car on the ground, you must set `belowRadar=false`.

### Cheats (shortcuts)
In cheats, type:
  - `goto lsia`, `goto zancudo`, or `goto sandyshores` to teleport to an airport
  - `<x>stars` to set wanted level. For example, `3stars` will set your wanted level to 3

---
## Changelog
### 0.3:
- restored `aircraftOnly` option
### 0.2.5:
- implemented below radar setting; jets will not be scrambled if player's aircraft is < 100 meters above the ground
### 0.2.4:
- make pursuing jets spawn **behind** player instead of in front, by default
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