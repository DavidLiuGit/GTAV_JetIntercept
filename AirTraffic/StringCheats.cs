using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using GTA;
using GTA.Math;
using GTA.UI;

namespace AirTraffic
{
	public class StringCheats
	{
		static StringCheats()
		{
			teleportTargetValueArr = (int[])Enum.GetValues(typeof(teleportTargets));
			teleportTargetStrArr = teleportTargetValueArr.Select(i => Enum.GetName(typeof(teleportTargets), i)).ToArray();
		}

		public static void handleKeyDown(KeyEventArgs e)
		{
			// detect teleport cheat strings:
			for (int i = 0; i < teleportTargetStrArr.Length; i++ )
			{
				if (Game.WasCheatStringJustEntered("goto " + teleportTargetStrArr[i]))
				{
					teleportTo(teleportTargetValueArr[i]);
					return;
				}
			}

			// detect wanted level cheat strings:
			for (int i = 0; i <= 5; i++)			// iterate over numbers 0 to 5
			{
				if (Game.WasCheatStringJustEntered(i + "stars"))
				{
					Game.Player.WantedLevel = i;
					return;
				}
			}
		}


		#region methods
		static int[] teleportTargetValueArr;
		static string[] teleportTargetStrArr;
		enum teleportTargets
		{
			lsia,
			zancudo,
			sandyshores,
			carrier
		}
		public static void teleportTo(int teleportTargetValue)
		{
			switch (teleportTargetValue)
			{
				case (int)teleportTargets.lsia:
					Game.Player.Character.Position = Airports.lsia.position; break;

				case (int)teleportTargets.zancudo:
					Game.Player.Character.Position = Airports.zancudo.position; break;

				case (int)teleportTargets.sandyshores:
					Game.Player.Character.Position = Airports.sandyShores.position; break;

				case (int)teleportTargets.carrier:
					break;		// TODO: find carrier position & properties
			}
		}



		#endregion
	}
}
