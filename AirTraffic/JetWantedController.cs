using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTA;
using GTA.Math;
using GTA.Native;


namespace AirTraffic
{
	class JetWantedController : TrafficController
	{
		#region properties
		protected int[] numJetsByWantedLevel;
		protected bool _aircraftOnly;
		protected bool _belowRadar;
		protected float _spawnDistance;
		protected const float _radarHeight = 100f;
		protected CustomWeaponJet[] _jets;
		#endregion


		public JetWantedController(ScriptSettings ss)
			: base(ss)
		{
			// read in settings for Jets
			string section = "JetsWanted";
			_jets = readCwjFromString(ss.GetValue<string>(section, "models", "lazer,hydra"));
			numJetsByWantedLevel = new int[] {
				0, 0, 0,											// 0, 1, and 2 stars respectively
				ss.GetValue<int>(section, "numJets3stars", 0),		// 3 stars
				ss.GetValue<int>(section, "numJets4stars", 1),		// 4 stars
				ss.GetValue<int>(section, "numJets5stars", 3),		// 5 stars
			};
			_aircraftOnly = ss.GetValue<bool>(section, "aircraftOnly", true);
			_drawBlip = ss.GetValue<bool>(section, "blip", true);
			_spawnTime = ss.GetValue<int>(section, "respawnTime", 30);
			_spawnDistance = ss.GetValue<float>(section, "spawnDistance", 500f);
			_belowRadar = ss.GetValue<bool>(section, "belowRadar", true);

			_minHeight = 300f;
			_maxHeight = 1500f;
			_maxDistance = 1000f;
			_lastVehicleSpawnTime = 0;
		}



		public override void onTick()
		{
			int currTime = Game.GameTime;

			// check if each spawned vehicle is still driveable
			_spawnedVehicles.RemoveAll(veh =>
			{
				if (!keepVehicle(veh))
				{
					vehicleDestructor(veh);		// invoke destructor
					return true;
				}
				return false;
			});

			// determine if we need to spawn more jets
			int activeJets = _spawnedVehicles.Count;
			int jetsNeeded = determineJetsNeeded();

			// if there are too many jets, dismiss extra jets
			if (activeJets > jetsNeeded)
			{
				for (int i = activeJets; i > jetsNeeded; i--)
				{
					vehicleDestructor(_spawnedVehicles[0], false);
					_spawnedVehicles.RemoveAt(0);
				}
			}
			
			// if more jets are needed:
			else if (activeJets < jetsNeeded && !belowRadar(Game.Player.Character)
				&& currTime > _lastVehicleSpawnTime + _spawnTime * 1000)
			{
				for (int i = activeJets; i < jetsNeeded; i++)	// assumes spawn is successful to avoid infinite loop
				{
					Vehicle veh = spawnAirTraffic();
					if (veh != null) _spawnedVehicles.Add(veh);
					_lastVehicleSpawnTime = currTime;
				}
			}
		}



		protected override Vehicle spawnAirTraffic()
		{
			// randomly select a jet to spawn
			CustomWeaponJet selectedJet = _jets[rng.Next(0, _jets.Length)];

			// determine the position and orientation to spawn the vehicle
			Vector3 spawnPos = getSpawnPosition();
			float spawnHeading = (float)rng.NextDouble() * 360f;

			// spawn vehicle
			Vehicle veh = World.CreateVehicle(selectedJet.model, spawnPos, spawnHeading);
			if (veh == null)
			{
				string modelName = selectedJet.modelName;
				GTA.UI.Notification.Show("~r~Jet Intercept: unable to spawn vehicle: " + modelName);
				return null;
			}

			// configure vehicle
			configureVehicle(veh);

			// spawn pilot in vehicle
			Ped pilot = spawnPilotInVehicle(veh);

			// give the pilot a task
			pilotTasking(veh, pilot);

			// if applicable, make pilot switch weapon
			if (selectedJet.weaponName != "")
			{
				bool pilotSwitchedWeapon = pilotSetVehicleWeapon(pilot, selectedJet.weaponHash);
				if (!pilotSwitchedWeapon)		// if unsuccessful
					GTA.UI.Notification.Show("~r~Jet Intercept: pilot of " + selectedJet.modelName +
						" unable to switch to weapon " + selectedJet.weaponName);
			}


			// draw blip on vehicle
			if (_drawBlip)
				drawCustomBlip(veh);

			return veh;
		}



		/// <summary>
		/// Determine the number of jets needed.
		/// </summary>
		/// <returns></returns>
		protected int determineJetsNeeded()
		{
			int jetsNeeded = numJetsByWantedLevel[Game.Player.WantedLevel];

			// if aircraftOnly is true:
			if (_aircraftOnly)
			{
				// get the player's current vehicle, if any
				Vehicle playerVeh = Game.Player.Character.CurrentVehicle;
				if (playerVeh == null) jetsNeeded = 0;
				else if (!playerVeh.Model.IsPlane && !playerVeh.Model.IsHelicopter) jetsNeeded = 0;
			}

			return jetsNeeded;
		}



		protected override bool keepVehicle(Vehicle veh)
		{
			// check if vehicle still driveable, and pilot alive
			if (!veh.IsDriveable || veh.Driver == null || veh.Driver.IsDead)
				return false;

			return true;
		}



		protected override void configureVehicle(Vehicle veh)
		{
			base.configureVehicle(veh);

			// place the attacking jet behind the player
			Vector3 playerPos = Game.Player.Character.Position;
			Vector3 spawnPos = playerPos + Game.Player.Character.ForwardVector * -_spawnDistance;
			spawnPos = spawnPos.Around((float)rng.NextDouble() * (_spawnDistance * 0.25f));
			spawnPos.Z = Math.Max(300f, playerPos.Z + rng.Next(-100, 100));		// enforce minimum spawn altitude
			veh.Position = spawnPos;

			// orient the attacking jet towards the player
			veh.Heading = Game.Player.Character.Heading;

			// set the attacking jet's speed to 200 kmh or player's current speed, whichever is higher
			veh.ForwardSpeed = Math.Max(200f, Game.Player.Character.Speed);
		}



		protected override Blip drawCustomBlip(Vehicle veh)
		{
			Blip blip = veh.AddBlip();
			blip.Sprite = BlipSprite.Jet;
			blip.Color = BlipColor.Red;
			return blip;
		}



		protected override Ped spawnPilotInVehicle(Vehicle veh)
		{
			Ped p = veh.CreatePedOnSeat(VehicleSeat.Driver, PedHash.Swat01SMY);
			p.FiringPattern = FiringPattern.FullAuto;
			p.DrivingStyle = DrivingStyle.Rushed;
			p.RelationshipGroup = (RelationshipGroup)0xA49E591C;
			return p;
		}



		protected override void pilotTasking(Vehicle veh, Ped pilot)
		{
			pilot.Task.FightAgainst(Game.Player.Character);
			pilot.AlwaysKeepTask = true;
		}



		protected void pilotOnTick(Vehicle jet, Ped pilot, Ped player)
		{
			// if player's jet is BEHIND the attacking jet
			if (Vector3.Dot(pilot.Position - player.Position, player.Rotation) > 0.0f)
			{
				// if the attacking jet is also facing away from the player, task it with chasing the player
				if (Vector3.Dot(pilot.Rotation, player.Rotation) > 0.0f)
				{
					pilot.Task.ChaseWithPlane(player, Vector3.Zero);
					GTA.UI.Screen.ShowHelpTextThisFrame("Player behind pilot. Retasking.");
				}
			}

			// otherwise, task pursuing pilot with attacking player
			pilot.Task.FightAgainst(Game.Player.Character);
		}


		/// <summary>
		/// Determine whether the player is below radar
		/// </summary>
		/// <param name="player"></param>
		/// <returns><c>true</c> if player is below radar</returns>
		protected bool belowRadar(Entity player)
		{
			if (!_belowRadar)
				return false;

			// if player's height above ground is less than radar height, then player is below radar
			if (player.HeightAboveGround < _radarHeight)
				return true;

			// if player is below sea level, then player is below radar
			else if (player.Position.Z < 0f)
				return true;

			return false;
		}



		protected CustomWeaponJet[] readCwjFromString(string models)
		{
			// split the string on comma delimiter, then generate hash from each model name
			List<string> modelStrings = models.Split(',').ToList();

			// custom weapons: split each modelString on colon ":"
			CustomWeaponJet[] cwj = new CustomWeaponJet[modelStrings.Count];
			for (int i = 0; i < modelStrings.Count; i++)
			{
				List<string> modelStringSplit = modelStrings[i].Trim().Split(':').ToList();
				cwj[i] = new CustomWeaponJet()
				{
					modelString = modelStrings[i],
					modelName = modelStringSplit[0],
					model = Game.GenerateHash(modelStringSplit[0]),
					weaponName = modelStringSplit.Count >= 2 ? modelStringSplit[1] : "",
					weaponHash = modelStringSplit.Count >= 2 ? (Hash)Game.GenerateHash(modelStringSplit[1]) : (Hash)0
				};
			}

			return cwj;
		}



		protected bool pilotSetVehicleWeapon(Ped pilot, Hash weaponHash)
		{
			// set pilot's vehicle weapon
			return Function.Call<bool>(Hash.SET_CURRENT_PED_VEHICLE_WEAPON, pilot, weaponHash);
		}
	}



	public struct CustomWeaponJet
	{
		public string modelString;

		public Model model;
		public string modelName;

		public Hash weaponHash;
		public string weaponName;
	}
}
