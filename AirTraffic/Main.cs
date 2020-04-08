// You must download and use Scripthook V Dot Net Reference (LINKS AT BOTTOM OF THE TEMPLATE)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using GTA;
using GTA.Native;
using GTA.Math;
using GTA.UI;


namespace ModName // !!!! IMPORTANT REPLACE THIS WITH YOUR MODS NAME !!!!
{
	public class Main : Script
	{
		// You can set your mod information below! Be sure to do this!
		bool firstTime = true;
		string ModName = "MOD NAME";
		string Developer = "YOUR NAME";

		public Main()
		{
			Tick += onTick;
			KeyDown += onKeyDown;
			Interval = 1;
		}

		private void onTick(object sender, EventArgs e)
		{
			if (firstTime) // if this is the users first time loading the mod, this information will appear
			{
				Notification.Show(ModName + " by " + Developer + " Loaded");
				firstTime = false;
			}
			// If the user has used the current mod version before, the text (and code) above will not appear

			// You can now begin your code here!
			Game.Player.Character.IsInvincible = true; // The character will be invincible if this script is active



			// ------------- ANY CODE PLACED ABOVE THIS LINE WILL HAPPEN WITH EVERY TICK (1 MS) OF THE SCRIPT -----------------
		}

		private void onKeyDown(object sender, KeyEventArgs e)
		{

		}

	}
}

namespace AirTraffic
{
	public class Main
	{
		// Nothing goes here
	}
}
// Useful Links
// All Vehicles - https://pastebin.com/uTxZnhaN
// All Player Models - https://pastebin.com/i5c1zA0W
// All Weapons - https://pastebin.com/M3kD9pnJ
// GTA V ScriptHook V Dot Net - https://www.gta5-mods.com/tools/scripthookv-net