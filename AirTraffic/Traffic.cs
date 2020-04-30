using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTA;
using GTA.Math;



namespace AirTraffic
{
	public abstract class TrafficController
	{
		#region properties
		// general
		protected int _lastVehicleSpawnTime;
		protected Random rng = new Random();

		// plane traffic
		protected Model[] _models;
		protected Dictionary<Model, string> _modelDict;
		protected int _spawnTime;
		protected float _minHeight, _maxHeight, _maxDistance;
		protected bool _drawBlip;

		// references
		protected List<Vehicle> _spawnedVehicles = new List<Vehicle>();
		protected List<Airport> _airports;
		#endregion




		#region constructor
		public TrafficController(ScriptSettings ss)
		{
			_lastVehicleSpawnTime = -9999;
			_airports = new List<Airport>();
		}


		public virtual void onTick()
		{
			int currTime = Game.GameTime;

			// determine if we need to spawn a plane
			if (currTime > _lastVehicleSpawnTime + _spawnTime * 1000)
			{
				Vehicle veh = spawnAirTraffic();
				if (veh != null) _spawnedVehicles.Add(veh);
				_lastVehicleSpawnTime = currTime;
			}

			// check if each spawned vehicle is still driveable & close enough to player
			_spawnedVehicles.RemoveAll(veh =>
			{
				if (!keepVehicle(veh))
				{
					vehicleDestructor(veh);
					return true;
				}
				return false;
			});
		}



		public virtual void destructor(bool force = false)
		{
			foreach (Vehicle veh in _spawnedVehicles)
				vehicleDestructor(veh, force);
		}
		#endregion




		#region helpers
		protected virtual bool keepVehicle(Vehicle veh)
		{
			// check if vehicle is not null and exists
			if (veh == null || !veh.Exists())
				return false;

			// check if vehicle still driveable, and pilot alive
			if (!veh.IsDriveable || veh.Driver == null || veh.Driver.IsDead)
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
					if (_drawBlip) veh.AttachedBlip.Delete();
					veh.Driver.Delete();
					veh.Delete();
				}

				// otherwise, task pilot to flee, and mark the pilot and vehicle as no longer needed
				else
				{
					if (_drawBlip) veh.AttachedBlip.Delete();
					veh.Driver.Task.FleeFrom(Game.Player.Character);
					veh.Driver.MarkAsNoLongerNeeded();
					veh.MarkAsNoLongerNeeded();
				}

			}
			catch { }
		}



		protected virtual Vehicle spawnAirTraffic()
		{
			Model selectedModel = _models[rng.Next(0, _models.Length)];

			// determine the position and orientation to spawn the vehicle
			Vector3 spawnPos = getSpawnPosition();
			float spawnHeading = (float)rng.NextDouble() * 360f;

			// spawn vehicle
			Vehicle veh = World.CreateVehicle(selectedModel, spawnPos, spawnHeading);
			if (veh == null)
			{
				string modelName = _modelDict[selectedModel];
				GTA.UI.Notification.Show("~r~Air Traffic: unable to spawn vehicle: " + modelName);
				return null;
			}

			// configure vehicle
			configureVehicle(veh);

			// spawn pilot in vehicle
			Ped pilot = spawnPilotInVehicle(veh);

			// give the pilot a task
			pilotTasking(veh, pilot);

			// draw blip on vehicle
			if (_drawBlip)
				drawCustomBlip(veh);

			return veh;
		}



		protected virtual Vector3 getSpawnPosition()
		{
			float spawnAltitude = (float)rng.NextDouble() * (_maxHeight - _minHeight) + _minHeight;
			float spawnDistance = (float)rng.NextDouble() * (_maxDistance / 2) + 150f;
			Vector3 spawnPos = Game.Player.Character.Position.Around(spawnDistance);
			spawnPos.Z = spawnAltitude;
			return spawnPos;
		}



		protected virtual void configureVehicle(Vehicle veh)
		{
			veh.ForwardSpeed = 50f;
			veh.IsEngineRunning = true;
			veh.LandingGearState = VehicleLandingGearState.Retracted;
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



		protected virtual Ped spawnPilotInVehicle(Vehicle veh)
		{
			Ped pilot = veh.CreatePedOnSeat(VehicleSeat.Driver, PedHash.Pilot01SMY);
			pilot.FiringPattern = FiringPattern.FullAuto;
			return pilot;
		}



		protected virtual Model[] readModelsFromString(string models)
		{
			return models.Split(',').ToList().Select(model => (Model)Game.GenerateHash(model.Trim())).ToArray();
		}


		protected abstract void pilotTasking(Vehicle veh, Ped pilot);
		#endregion
	}
}
