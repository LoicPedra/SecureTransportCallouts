using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSPD_First_Response.Mod.API;
using Rage;

namespace SecureTransportCallouts
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("Plugin " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " has been initialised.");

            Game.DisplayNotification("SecureTransportCallouts loaded!");
        }

        public override void Finally()
        {
            Game.LogTrivial("Plugin " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " has been cleaned up.");
        }
        private static void OnOnDutyStateChangedHandler(bool OnDuty)
        {
            if (OnDuty)
            {
                RegisterCallouts();
            }
        }
        private static void RegisterCallouts()
        {
            Functions.RegisterCallout(typeof(SecureTransportCallouts.SecureTransport1));
        }
    }
}
