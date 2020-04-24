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
		#endregion


		public JetWantedController(ScriptSettings ss)
			: base(ss)
		{
			// read in settings for Jets
			string section = "JetsWanted";
			_models = readModelsFromString(ss.GetValue<string>(section, "models", "lazer,hydra"));
			_modelDict = readModelsFromStringToDict(ss.GetValue<string>(section, "models", "lazer,hydra"));
			numJetsByWantedLevel = new int[] {
				0, 0, 0,											// 0, 1, and 2 stars respectively
				ss.GetValue<int>(section, "numJets3stars", 0),		// 3 stars
				ss.GetValue<int>(section, "numJets4stars", 1),		// 4 stars
				ss.GetValue<int>(section, "numJets5stars", 3),		// 5 stars
			};
			_aircraftOnly = ss.GetValue<bool>(section, "aircraftOnly", true);
			_drawBlip = ss.GetValue<bool>(section, "blip", true);
			_spawnTime = ss.GetValue<int>(section, "respawnTime", 30);

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
			int jetsNeeded = numJetsByWantedLevel[Game.Player.WantedLevel];

			// check if player is in a jet/heli
			Vehicle playerVeh = Game.Player.Character.CurrentVehicle;
			if (playerVeh == null) jetsNeeded = 0;
			else if (!playerVeh.Model.IsPlane && !playerVeh.Model.IsHelicopter) jetsNeeded = 0;

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
			else if (activeJets < jetsNeeded && currTime > _lastVehicleSpawnTime + _spawnTime * 1000)
			{
				for (int i = activeJets; i < jetsNeeded; i++)	// assumes spawn is successful to avoid infinite loop
				{
					Vehicle veh = spawnAirTraffic();
					if (veh != null) _spawnedVehicles.Add(veh);
					_lastVehicleSpawnTime = currTime;
				}
			}
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
			Vector3 spawnPos = playerPos + Game.Player.Character.ForwardVector * -rng.Next(250, 400);
			spawnPos = spawnPos.Around((float)rng.NextDouble() * 200f);
			spawnPos.Z = Math.Max(300f, playerPos.Z + rng.Next(-100, 100));
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
	}
}
