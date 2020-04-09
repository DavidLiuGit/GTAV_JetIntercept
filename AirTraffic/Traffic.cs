using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTA;
using GTA.Math;



namespace AirTraffic
{
	public class TrafficController
	{
		#region properties
		// general
		private int _lastPlaneSpawnTime;
		private Random rng = new Random();

		// plane traffic
		private Model[] _models;
		private int _spawnTime;
		private float _minHeight, _maxHeight, _maxDistance;
		private bool _drawBlip;

		// references
		private List<Vehicle> _spawnedVehicles = new List<Vehicle>();
		#endregion




		#region constructor
		public TrafficController(ScriptSettings ss)
		{
			_lastPlaneSpawnTime = Game.GameTime;

			// read in settings for Planes
			string section = "Planes";
			_models = readModelsFromString(ss.GetValue<string>(section, "models", "lazer"));
			_spawnTime = ss.GetValue<int>(section, "spawnTime", 60);
			_minHeight = ss.GetValue<float>(section, "minHeight", 300f);
			_maxHeight = ss.GetValue<float>(section, "maxHeight", 1000f);
			_maxDistance = ss.GetValue<float>(section, "maxDistance", 2000f);
			_drawBlip = ss.GetValue<bool>(section, "blip", true);
		}


		public void onTick()
		{
			int currTime = Game.GameTime;

			// determine if we need to spawn a plane
			if (currTime > _lastPlaneSpawnTime + _spawnTime * 1000)
			{
				_spawnedVehicles.Add(spawnAirTraffic());
				_lastPlaneSpawnTime = currTime;
			}
		}



		public void destructor(bool force = false)
		{
			// if destroying by force, delete everything right away
			if (force)
			{
				foreach(Vehicle veh in _spawnedVehicles){
					veh.Driver.Delete();
					veh.Delete();
				}
			}
		}
		#endregion




		#region helpers
		private Vehicle spawnAirTraffic()
		{
			Model selectedModel = _models[rng.Next(0, _models.Length)];

			// determine the position and orientation to spawn the vehicle
			float spawnAltitude = (float) rng.NextDouble() * (_maxHeight - _minHeight) + _minHeight;
			float spawnDistance = (float) rng.NextDouble() * (_maxDistance / 2) + 50f;
			Vector3 spawnPos = Game.Player.Character.Position.Around(spawnDistance);
			spawnPos.Z = spawnAltitude;
			float spawnHeading = (float)rng.NextDouble() * 360f;

			// spawn vehicle
			Vehicle veh = World.CreateVehicle(selectedModel, spawnPos, spawnHeading);
			veh.IsEngineRunning = true;
			veh.ForwardSpeed = 50f;
			veh.LandingGearState = VehicleLandingGearState.Retracted;

			// spawn pilot in vehicle
			spawnPilotInVehicle(veh);

			// draw blip on vehicle
			if (_drawBlip)
			{
				veh.AddBlip();
				veh.AttachedBlip.Alpha = 120;
				veh.AttachedBlip.Color = BlipColor.Grey;
				veh.AttachedBlip.Scale = 0.75f;
				veh.AttachedBlip.Sprite = BlipSprite.Plane;
			}

			return veh;
		}



		private void spawnPilotInVehicle(Vehicle veh)
		{
			Ped pilot = veh.CreatePedOnSeat(VehicleSeat.Driver, PedHash.Pilot01SMY);
			pilot.FiringPattern = FiringPattern.FullAuto;
			pilot.Task.FightAgainst(Game.Player.Character);
		}



		private Model[] readModelsFromString(string models)
		{
			// split the string on comma delimiter, then generate hash from each model name
			return models.Split(',').ToList().Select(model => (Model)Game.GenerateHash(model.Trim())).ToArray();
		}
		#endregion
	}
}
