using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Player;
using static PlayerGraphics;

namespace Witness
{
    public static class EXRelativeHooks
    {
        
        public static void Init()
        {
            On.PlayerGraphics.ctor += PlayerGraphics_ctor;
        }

        private static void PlayerGraphics_ctor(On.PlayerGraphics.orig_ctor orig, PlayerGraphics self, PhysicalObject ow)
        {
            orig(self, ow);
            if (!self.player.IsExile(out var Exi))
            {
                return;
            }

            Exi.RecreateTailIfNeeded(self);

            Exi.SetupColors(self);

            Exi.SetupTailTexture(Exi);
        }


    }
}
