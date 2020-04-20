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
			_models = readModelsFromString(ss.GetValue<string>(section, "models", "lazer"));
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

			// if player is not in aircraft and _aircraftOnly is true, do nothing
			if (_aircraftOnly){
				Vehicle playerVeh = Game.Player.Character.CurrentVehicle;
				// if player is not in a vehicle, do nothing
				if (playerVeh == null) return;
				// if player's vehicle is not an aircraft, do nothing
				else if (!playerVeh.Model.IsPlane && !playerVeh.Model.IsHelicopter) return;
			}
			
			// determine if we need to spawn more jets
			int activeJets = _spawnedVehicles.Count;
			int jetsNeeded = numJetsByWantedLevel[Game.Player.WantedLevel];

			// if more jets are needed:
			if (activeJets < jetsNeeded && currTime > _lastVehicleSpawnTime + _spawnTime * 1000)
			{
				for (int i = activeJets; i < jetsNeeded; i++)	// assumes spawn is successful to avoid infinite loop
				{
					Vehicle veh = spawnAirTraffic();
					if (veh != null) _spawnedVehicles.Add(veh);
					_lastVehicleSpawnTime = currTime;
				}
			}

			// if there are too many jets, dismiss extra jets
			else if (activeJets > jetsNeeded)
			{
				for (int i = activeJets; i >= jetsNeeded; i--)
				{
					vehicleDestructor(_spawnedVehicles[i - 1], false);
					_spawnedVehicles.RemoveAt(i - 1);
				}
			}
			
			// for each active jet/pilot, invoke its onTick
			//foreach (Vehicle veh in _spawnedVehicles)
			//{
			//	pilotOnTick(veh, veh.Driver, Game.Player.Character);
			//}
		}


		protected override bool keepVehicle(Vehicle veh)
		{
			// check if vehicle still driveable, and pilot alive
			if (!veh.IsDriveable || veh.Driver.IsDead)
				return false;

			return true;
		}



		protected override void configureVehicle(Vehicle veh)
		{
			base.configureVehicle(veh);
			veh.ForwardSpeed = 120f;
		}



		protected override Blip drawCustomBlip(Vehicle veh)
		{
			Blip blip = base.drawCustomBlip(veh);
			blip.Sprite = BlipSprite.Jet;
			blip.Color = BlipColor.Red;
			return blip;
		}



		protected override Ped spawnPilotInVehicle(Vehicle veh)
		{
			Ped p = veh.CreatePedOnSeat(VehicleSeat.Driver, PedHash.Swat01SMY);
			p.FiringPattern = FiringPattern.FullAuto;
			p.Task.FightAgainst(Game.Player.Character);
			p.AlwaysKeepTask = true;
			p.DrivingStyle = DrivingStyle.Rushed;
			p.RelationshipGroup = (RelationshipGroup)0xA49E591C;
			return p;
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
