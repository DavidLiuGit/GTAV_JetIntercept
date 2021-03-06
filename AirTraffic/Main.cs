﻿// You must download and use Scripthook V Dot Net Reference (LINKS AT BOTTOM OF THE TEMPLATE)
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


namespace AirTraffic
{
	public class Main : Script
	{
		// You can set your mod information below! Be sure to do this!
		bool firstTime = true;
		string ModName = "Custom Air Traffic";
		string Developer = "iLike2Teabag";


		#region objReferences
		ScriptSettings ss;
		TrafficController jetWantedCtrl;
		#endregion


		public Main()
		{
			Tick += onTick;
			KeyDown += onKeyDown;
			Interval = 1000;
			Aborted += onAbort;
		}



		private void onTick(object sender, EventArgs e)
		{
			if (firstTime) // if this is the users first time loading the mod, this information will appear
			{
				Notification.Show(ModName + " by " + Developer + " Loaded");
				firstTime = false;

				ss = base.Settings;
				jetWantedCtrl = new JetWantedController(ss);
				new StringCheats();
			}


			else
			{
				jetWantedCtrl.onTick();
			}
		}




		private void onKeyDown(object sender, KeyEventArgs e)
		{
			StringCheats.handleKeyDown(e);
		}



		private void onAbort(object sender, EventArgs e)
		{
			jetWantedCtrl.destructor(true);
		}
	}
}
