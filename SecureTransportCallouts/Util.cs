using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Drawing;

namespace SecureTransportCallouts
{
    class Util
    {

        public static Blip AttachBlipToPed(Ped ped)
        {
            return new Blip(ped);
        }

        public static Blip AttachBlipToPed(Ped ped, Color color)
        {
            Blip blip = new Blip(ped);
            blip.Color = color;

            return blip;
        }

    }
}
