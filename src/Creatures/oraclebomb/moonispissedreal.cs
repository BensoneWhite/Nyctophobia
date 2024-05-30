using System;
using System.Collections.Generic;
using MoreSlugcats;
using RWCustom;
using UnityEngine;


public abstract class OracleBomberHooks
{
    public static void Apply()
    {

        On.OracleSwarmer.Update += glue;

    }

    private static void glue(On.OracleSwarmer.orig_Update orig, OracleSwarmer self, bool eu)
    {
        orig(self,eu);
    }
    

}
