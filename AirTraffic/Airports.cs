using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTA;
using GTA.Math;



namespace AirTraffic
{
	public struct Airport
	{
		// properties
		public string name;

		// position
		public float height;
		public Vector3 position;

		// runways
		public struct Runway
		{
			public Vector3 startPos, endPos;
		}
		public Runway[] runways;
	}



	public class Airports
	{
		static Airports()
		{
			zancudo = defineZancudo();
		}


		// Fort Zancudo
		public static Airport zancudo;
		private static Airport defineZancudo ()
		{
			float ZancudoHeight = 32f;
			Airport zancudo = new Airport
			{
				name = "Fort Zancudo",
				height = ZancudoHeight,
				position = new Vector3(-2283f, 3121f, ZancudoHeight),
				runways = new Airport.Runway[]
				{
					new Airport.Runway {
						startPos = new Vector3(-2681.8f, 3247.8f, ZancudoHeight),
						endPos = new Vector3(-2031.8f, 2872.4f, ZancudoHeight),
					}
				}
			};

			return zancudo;
		}
	}
}
