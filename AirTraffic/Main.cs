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
		TrafficController jetCtrl;
		TrafficController planeCtrl;
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
				jetCtrl = new JetTrafficController(ss);
				planeCtrl = new PlaneTrafficController(ss);
				jetWantedCtrl = new JetWantedController(ss);
			}


			else
			{
				jetCtrl.onTick();
				planeCtrl.onTick();
				jetWantedCtrl.onTick();
			}
		}




		private void onKeyDown(object sender, KeyEventArgs e)
		{

		}



		private void onAbort(object sender, EventArgs e)
		{
			jetCtrl.destructor(true);
			planeCtrl.destructor(true);
			jetWantedCtrl.destructor(true);
		}
	}
}
