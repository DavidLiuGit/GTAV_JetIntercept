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
			lsia = defineLsia();
			sandyShores = defineSandyShores();
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


		// LSIA
		public static Airport lsia;
		private static Airport defineLsia()
		{
			float lsiaHeight = 13f;
			Airport lsia = new Airport
			{
				name = "Los Santos International Airport",
				height = lsiaHeight,
				position = new Vector3(-1570f, -2747f, lsiaHeight),
				runways = new Airport.Runway[] { 
					new Airport.Runway {
						startPos = new Vector3(-1356f, -2245f, lsiaHeight),
						endPos = new Vector3(-1625f, -2711f, lsiaHeight)
					},
					new Airport.Runway {
						startPos = new Vector3(-963.2f, -3165.4f, lsiaHeight),
						endPos = new Vector3(-1535.5f, -2833.9f, lsiaHeight),
					},
					new Airport.Runway {
						startPos = new Vector3(-1623.8f, -2977.5f, lsiaHeight),
						endPos = new Vector3(1168.9f, -3240.2f, lsiaHeight),
					}
				}
			};

			return lsia;
		}


		// Sandy shores
		public static Airport sandyShores;
		private static Airport defineSandyShores()
		{
			float sandyShoresHeight = 40.35f;
			Airport sandyShores = new Airport
			{
				name = "Sandy Shores Airfield",
				height = sandyShoresHeight,
				position = new Vector3(1693f, 3247f, sandyShoresHeight),
				runways = new Airport.Runway[] { 
					new Airport.Runway {
						startPos = new Vector3(1084.2f, 3083.5f, sandyShoresHeight),
						endPos = new Vector3(1693f, 3247.6f, sandyShoresHeight)
					},
					new Airport.Runway {
						startPos = new Vector3(1392f, 2991.3f, sandyShoresHeight),
						endPos = new Vector3(1604f, 3203f, sandyShoresHeight),
					},
				}
			};

			return sandyShores;
		}
	}
}
