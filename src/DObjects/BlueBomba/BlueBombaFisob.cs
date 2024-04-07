﻿using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace Nyctophobia;

public class BlueBombaFisob : Fisob
{
    public BlueBombaFisob() : base(NTEnums.AbstractObjectType.Bluebomba)
    {
        Icon = new SimpleIcon("Symbol_StunBomb", Color.cyan);

        RegisterUnlock(NTEnums.SandboxUnlock.BlueBomba);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData entitySaveData, SandboxUnlock unlock)
    {
        var result = new BlueBombaAbstract(world, entitySaveData.Pos, entitySaveData.ID);
         
        return result;
    }

    private static readonly BlueBombaProperties BlueBombaProperties = new();

    public override ItemProperties Properties(PhysicalObject forObject) => BlueBombaProperties;
}
