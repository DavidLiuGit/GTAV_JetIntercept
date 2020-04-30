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
#### Models
A comma-separated list of the jet models to be spawned. Add-on jet model names are allowed. Models are selected randomly. **Custom weapon are supported**, provided you've formatted this setting correctly.

**Optional:** to make a jet use a custom weapon, format a model name like so: `[model_name]:[weapon_name]`.  
For example, if you'd like to use [Voltrock & SkylineGTRFreak's F/A-18F Super Hornet](https://www.gta5-mods.com/vehicles/f18f-super-hornet-addon) as a intercepting jet, but would like the pilot to use his cannon: `fa18f:VEHICLE_WEAPON_FA18F_GUN`.  
Separate your models list using commas like so: `models=fa18f:VEHICLE_WEAPON_FA18F_GUN,lazer,hydra`.  

`weapon_name` is found in the jet's `handling.meta` file, under `<Item type="CVehicleWeaponHandlingData">`.
```XML
<SubHandlingData>
  <!-- ... -->
  <Item type="CVehicleWeaponHandlingData">
    <uWeaponHash>
      <Item>VEHICLE_WEAPON_FA18F_GUN</Item>     <!-- these are the weapon names -->
      <Item>VEHICLE_WEAPON_FA18F_ROCKET</Item>
      <Item />
    </uWeaponHash>
    <!-- ... -->
```

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
## Development
[Source code on GitHub](https://github.com/DavidLiuGit/GTAV_JetIntercept)  
[Release on GTA5 Mods](https://www.gta5-mods.com/scripts/jets-when-wanted)

### Changelog
#### 1.0.1:
- bugfix: disable AI pilot switching weapon after being assigned their custom weapon
#### 1.0:
- enabled custom weapons for intercepting jets
  - read instructions in Settings section!
#### 0.3:
- restored `aircraftOnly` option
#### 0.2.5:
- implemented below radar setting; jets will not be scrambled if player's aircraft is < 100 meters above the ground
#### 0.2.4:
- make pursuing jets spawn **behind** player instead of in front, by default
#### 0.2.3:
- added INI setting for spawn distance; default value 500 meters
#### 0.2.2 (unreleased):
- added cheats to get you back into a dogfight quickly after death. In cheats, type:
  - `goto lsia`, `goto zancudo`, or `goto sandyshores` to teleport to an airport
  - `<x>stars` to set wanted level. For example, `3stars` will set your wanted level to 3
#### 0.2.1:
- suppressed "jets needed" debug output
#### 0.2:
- renamed file to CustomWantedAirTraffic. Please remove CustomAirTraffic if you have already installed 0.1