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
		protected int _lastVehicleSpawnTime;
		protected Random rng = new Random();

		// plane traffic
		protected Model[] _models;
		protected int _spawnTime;
		protected float _minHeight, _maxHeight, _maxDistance;
		protected bool _drawBlip;

		// references
		protected List<Vehicle> _spawnedVehicles = new List<Vehicle>();
		#endregion




		#region constructor
		public TrafficController(ScriptSettings ss)
		{
			_lastVehicleSpawnTime = Game.GameTime;
		}


		public void onTick()
		{
			int currTime = Game.GameTime;

			// determine if we need to spawn a plane
			if (currTime > _lastVehicleSpawnTime + _spawnTime * 1000)
			{
				_spawnedVehicles.Add(spawnAirTraffic());
				_lastVehicleSpawnTime = currTime;
			}

			// check if each spawned vehicle is still driveable & close enough to player
			foreach (Vehicle veh in _spawnedVehicles)
			{
				if (!keepVehicle(veh))
					vehicleDestructor(veh);
			}
		}



		public void destructor(bool force = false)
		{
			foreach (Vehicle veh in _spawnedVehicles)
				vehicleDestructor(veh, force);
		}
		#endregion




		#region helpers
		protected virtual bool keepVehicle(Vehicle veh)
		{
			// check if vehicle still driveable, and pilot alive
			if (!veh.IsDriveable || veh.Driver.IsDead)
				return false;

			// check whether the vehicle's position is close enough
			if (veh.Position.DistanceTo2D(Game.Player.Character.Position) > _maxDistance)
				return false;

			return true;
		}


		protected virtual void vehicleDestructor(Vehicle veh, bool force = false)
		{
			try
			{
				// if destroying by force, delete all assets associated with the vehicle
				if (force)
				{
					veh.Driver.Delete();
					if (_drawBlip) veh.AttachedBlip.Delete();
					veh.Delete();
				}

				// otherwise, task pilot to flee, and mark the pilot and vehicle as no longer needed
				else
				{
					veh.Driver.Task.FleeFrom(Game.Player.Character);
					veh.Driver.MarkAsNoLongerNeeded();
					if (_drawBlip) veh.AttachedBlip.Delete();
					veh.MarkAsNoLongerNeeded();
				}
			}
			catch { }
		}



		protected virtual Vehicle spawnAirTraffic()
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
			veh.LandingGearState = VehicleLandingGearState.Retracted;
			configureVehicle(veh);

			// spawn pilot in vehicle
			spawnPilotInVehicle(veh);

			// draw blip on vehicle
			if (_drawBlip)
				drawCustomBlip(veh);

			return veh;
		}



		protected virtual void configureVehicle(Vehicle veh)
		{
			veh.ForwardSpeed = 50f;
		}



		protected virtual Blip drawCustomBlip(Vehicle veh)
		{
			Blip blip = veh.AddBlip();
			blip.Alpha = 120;
			blip.Color = BlipColor.Grey;
			blip.Scale = 0.75f;
			blip.Sprite = BlipSprite.Plane;
			return blip;
		}



		protected virtual void spawnPilotInVehicle(Vehicle veh)
		{
			Ped pilot = veh.CreatePedOnSeat(VehicleSeat.Driver, PedHash.Pilot01SMY);
			pilot.FiringPattern = FiringPattern.FullAuto;
			pilot.Task.FightAgainst(Game.Player.Character);
		}



		protected Model[] readModelsFromString(string models)
		{
			GTA.UI.Notification.Show("models: " + models);
			// split the string on comma delimiter, then generate hash from each model name
			return models.Split(',').ToList().Select(model => (Model)Game.GenerateHash(model.Trim())).ToArray();
		}
		#endregion
	}
}
