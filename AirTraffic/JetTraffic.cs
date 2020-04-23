using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using GTA;
using GTA.Math;
using GTA.UI;
using GTA.Native;


namespace AirTraffic
{
	class JetTrafficController : TrafficController
	{


		public JetTrafficController(ScriptSettings ss)
			: base(ss)
		{
			// read in settings for Jets
			string section = "Jets";
			_models = readModelsFromString(ss.GetValue<string>(section, "models", "lazer"));
			_spawnTime = ss.GetValue<int>(section, "spawnTime", 120);
			_minHeight = ss.GetValue<float>(section, "minHeight", 300f);
			_maxHeight = ss.GetValue<float>(section, "maxHeight", 1000f);
			_maxDistance = ss.GetValue<float>(section, "maxDistance", 3000f);
			_drawBlip = ss.GetValue<bool>(section, "blip", true);

			// configure airports
			_airports = new List<Airport> { Airports.zancudo, Airports.lsia };
		}


		protected override Blip drawCustomBlip(Vehicle veh)
		{
			Blip blip = base.drawCustomBlip(veh);
			blip.Sprite = BlipSprite.Jet;
			return blip;
		}


		protected override void configureVehicle(Vehicle veh)
		{
			base.configureVehicle(veh);
			veh.ForwardSpeed = 120f;
		}


		protected override void pilotTasking(Vehicle veh, Ped pilot)
		{
			// task pilot with landing at an airport
			Airport ap = _airports[rng.Next(0, _airports.Count)];
			Airport.Runway rw = ap.runways[rng.Next(0, ap.runways.Length)];
			pilot.Task.LandPlane(rw.startPos, rw.endPos, veh);
			Notification.Show("Jet landing at " + ap.name);
		}
	
	}





	class PlaneTrafficController : TrafficController
	{
		public PlaneTrafficController(ScriptSettings ss)
			: base(ss)
		{
			string section = "Planes";
			_models = readModelsFromString(ss.GetValue<string>(section, "models", "luxor"));
			_spawnTime = ss.GetValue<int>(section, "spawnTime", 90);
			_minHeight = ss.GetValue<float>(section, "minHeight", 500f);
			_maxHeight = ss.GetValue<float>(section, "maxHeight", 1000f);
			_maxDistance = ss.GetValue<float>(section, "maxDistance", 2000f);
			_drawBlip = ss.GetValue<bool>(section, "blip", true);

			// configure airports
			_airports = new List<Airport> { Airports.lsia, Airports.sandyShores };
		}



		protected override void configureVehicle(Vehicle veh)
		{
			base.configureVehicle(veh);
			veh.ForwardSpeed = 90f;
		}



		protected override void pilotTasking(Vehicle veh, Ped pilot)
		{
			// task pilot with landing at an airport
			Airport ap = _airports[rng.Next(0, _airports.Count)];
			Airport.Runway rw = ap.runways[rng.Next(0, ap.runways.Length)];
			pilot.Task.LandPlane(rw.startPos, rw.endPos, veh);

			Notification.Show("Plane landing at " + ap.name);
		}
	}
}
